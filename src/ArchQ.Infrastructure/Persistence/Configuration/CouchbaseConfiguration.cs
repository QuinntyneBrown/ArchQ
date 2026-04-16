namespace ArchQ.Infrastructure.Persistence.Configuration;

public class CouchbaseConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string BucketName { get; set; } = "archq";
}
