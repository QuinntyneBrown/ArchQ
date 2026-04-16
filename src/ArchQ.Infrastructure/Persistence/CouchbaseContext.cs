using Couchbase;
using Couchbase.KeyValue;
using Microsoft.Extensions.Options;
using ArchQ.Infrastructure.Persistence.Configuration;

namespace ArchQ.Infrastructure.Persistence;

public class CouchbaseContext : IAsyncDisposable
{
    private readonly CouchbaseConfiguration _config;
    private ICluster? _cluster;
    private IBucket? _bucket;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public CouchbaseContext(IOptions<CouchbaseConfiguration> options)
    {
        _config = options.Value;
    }

    private async Task EnsureConnectedAsync()
    {
        if (_cluster is not null) return;

        await _lock.WaitAsync();
        try
        {
            if (_cluster is not null) return;

            var clusterOptions = new ClusterOptions()
                .WithPasswordAuthentication(_config.Username, _config.Password);

            _cluster = await Cluster.ConnectAsync(_config.ConnectionString, clusterOptions);
            _bucket = await _cluster.BucketAsync(_config.BucketName);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IBucket> GetBucketAsync()
    {
        await EnsureConnectedAsync();
        return _bucket!;
    }

    public async Task<ICluster> GetClusterAsync()
    {
        await EnsureConnectedAsync();
        return _cluster!;
    }

    public async Task<IScope> GetScopeAsync(string slug)
    {
        var bucket = await GetBucketAsync();
        return bucket.Scope(slug);
    }

    public async Task<ICouchbaseCollection> GetCollectionAsync(string scope, string collectionName)
    {
        var bucket = await GetBucketAsync();
        return bucket.Scope(scope).Collection(collectionName);
    }

    public async ValueTask DisposeAsync()
    {
        if (_cluster is not null)
        {
            await _cluster.DisposeAsync();
        }

        _lock.Dispose();
    }
}
