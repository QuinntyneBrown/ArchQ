using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ArchQ.Infrastructure.Persistence;
using ArchQ.Infrastructure.Persistence.Configuration;
using Couchbase.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ArchQ.Cli.Handlers;

public class InitHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CouchbaseContext _couchbaseContext;
    private readonly CouchbaseBootstrapper _bootstrapper;
    private readonly IOptions<CouchbaseConfiguration> _config;
    private readonly ILogger<InitHandler> _logger;

    private static readonly string[] SystemCollections =
    [
        "tenants",
        "global_users",
        "verification_tokens",
        "refresh_tokens",
        "audit"
    ];

    public InitHandler(
        IHttpClientFactory httpClientFactory,
        CouchbaseContext couchbaseContext,
        CouchbaseBootstrapper bootstrapper,
        IOptions<CouchbaseConfiguration> config,
        ILogger<InitHandler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _couchbaseContext = couchbaseContext;
        _bootstrapper = bootstrapper;
        _config = config;
        _logger = logger;
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        var config = _config.Value;
        var baseUrl = ToManagementUrl(config.ConnectionString);

        _logger.LogInformation("Initializing Couchbase cluster at {BaseUrl}", baseUrl);

        var client = _httpClientFactory.CreateClient();

        // Step 1: Set data and index paths
        _logger.LogInformation("Configuring node storage paths...");
        await PostFormAsync(client, $"{baseUrl}/nodes/self/controller/settings", new Dictionary<string, string>
        {
            ["data_path"] = "/opt/couchbase/var/lib/couchbase/data",
            ["index_path"] = "/opt/couchbase/var/lib/couchbase/data"
        }, cancellationToken: cancellationToken);

        // Step 2: Enable services
        _logger.LogInformation("Enabling cluster services (kv, n1ql, index, fts)...");
        await PostFormAsync(client, $"{baseUrl}/node/controller/setupServices", new Dictionary<string, string>
        {
            ["services"] = "kv,n1ql,index,fts"
        }, cancellationToken: cancellationToken);

        // Step 3: Set memory quotas
        _logger.LogInformation("Setting memory quotas (data=512MB, index=256MB, fts=256MB)...");
        await PostFormAsync(client, $"{baseUrl}/pools/default", new Dictionary<string, string>
        {
            ["memoryQuota"] = "512",
            ["indexMemoryQuota"] = "256",
            ["ftsMemoryQuota"] = "256"
        }, cancellationToken: cancellationToken);

        // Step 4: Set admin credentials
        _logger.LogInformation("Setting administrator credentials (user={Username})...", config.Username);
        await PostFormAsync(client, $"{baseUrl}/settings/web", new Dictionary<string, string>
        {
            ["username"] = config.Username,
            ["password"] = config.Password,
            ["port"] = "SAME"
        }, cancellationToken: cancellationToken);

        // From here on, use Basic auth
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        // Step 5: Create bucket
        _logger.LogInformation("Creating bucket '{BucketName}'...", config.BucketName);
        var bucketResult = await PostFormAsync(client, $"{baseUrl}/pools/default/buckets", new Dictionary<string, string>
        {
            ["name"] = config.BucketName,
            ["bucketType"] = "couchbase",
            ["ramQuota"] = "256",
            ["flushEnabled"] = "0"
        }, cancellationToken: cancellationToken);

        if (!bucketResult)
        {
            _logger.LogWarning("Bucket creation returned an error; it may already exist. Continuing...");
        }

        // Step 6: Wait for bucket to be ready
        _logger.LogInformation("Waiting for bucket to become ready...");
        var bucketReady = await WaitForBucketReadyAsync(client, $"{baseUrl}/pools/default/buckets/{config.BucketName}", cancellationToken);
        if (!bucketReady)
        {
            _logger.LogError("Bucket did not become ready within the timeout period");
            return 1;
        }

        _logger.LogInformation("Bucket '{BucketName}' is ready", config.BucketName);

        // Step 7: Create system scope and collections via SDK
        _logger.LogInformation("Creating system scope and collections...");
        await _bootstrapper.EnsureSystemScopeAsync();

        // Step 8: Wait for collections to be queryable, then create system indexes
        _logger.LogInformation("Waiting for system collections to propagate...");
        await Task.Delay(5000, cancellationToken);

        _logger.LogInformation("Creating primary indexes on system collections...");
        var bucket = await _couchbaseContext.GetBucketAsync();
        var systemScope = bucket.Scope("system");

        foreach (var collection in SystemCollections)
        {
            var query = $"CREATE PRIMARY INDEX IF NOT EXISTS ON `{collection}`";
            try
            {
                await systemScope.QueryAsync<dynamic>(query, new QueryOptions().CancellationToken(cancellationToken));
                _logger.LogInformation("  Created primary index on system.{Collection}", collection);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "  Failed to create index on system.{Collection}; it may already exist", collection);
            }
        }

        _logger.LogInformation("Couchbase cluster initialization complete");
        return 0;
    }

    private async Task<bool> PostFormAsync(
        HttpClient client,
        string url,
        Dictionary<string, string> formData,
        CancellationToken cancellationToken)
    {
        using var content = new FormUrlEncodedContent(formData);

        try
        {
            var response = await client.PostAsync(url, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("POST {Url} returned {StatusCode}: {Body}", url, (int)response.StatusCode, body);
                return false;
            }

            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "POST {Url} failed", url);
            return false;
        }
    }

    private async Task<bool> WaitForBucketReadyAsync(HttpClient client, string bucketUrl, CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromSeconds(60);
        var interval = TimeSpan.FromSeconds(2);
        var elapsed = TimeSpan.Zero;

        while (elapsed < timeout)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var response = await client.GetAsync(bucketUrl, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync(cancellationToken);
                    using var doc = JsonDocument.Parse(json);

                    if (doc.RootElement.TryGetProperty("nodes", out var nodes))
                    {
                        var allHealthy = true;
                        foreach (var node in nodes.EnumerateArray())
                        {
                            if (node.TryGetProperty("status", out var status) &&
                                status.GetString() == "healthy")
                            {
                                continue;
                            }
                            allHealthy = false;
                            break;
                        }

                        if (allHealthy && nodes.GetArrayLength() > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (HttpRequestException)
            {
                // Bucket not ready yet
            }

            _logger.LogInformation("  Bucket not ready yet, waiting {Interval}s...", interval.TotalSeconds);
            await Task.Delay(interval, cancellationToken);
            elapsed += interval;
        }

        return false;
    }

    private static string ToManagementUrl(string connectionString)
    {
        // couchbase://host -> http://host:8091
        // couchbases://host -> https://host:18091
        // couchbase://host1,host2 -> use first host

        var uri = connectionString.Trim();

        if (uri.StartsWith("couchbases://", StringComparison.OrdinalIgnoreCase))
        {
            var host = uri["couchbases://".Length..].Split(',')[0].Trim('/');
            return $"https://{host}:18091";
        }

        if (uri.StartsWith("couchbase://", StringComparison.OrdinalIgnoreCase))
        {
            var host = uri["couchbase://".Length..].Split(',')[0].Trim('/');
            return $"http://{host}:8091";
        }

        // Fallback: assume it's already a management URL
        return uri.TrimEnd('/');
    }
}
