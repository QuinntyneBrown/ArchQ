namespace ArchQ.Core.Interfaces;

public interface ICouchbaseProvisioner
{
    Task ProvisionScopeAsync(string slug);
    Task CreateCollectionsAsync(string slug);
    Task CreateIndexesAsync(string slug);
}
