using BenchmarkDotNet.Attributes;
using ArchQ.Core.Entities;
using ArchQ.Infrastructure.Persistence.Repositories;
using ArchQ.PerformanceTests.Infrastructure;

namespace ArchQ.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class UserBenchmarks : CouchbaseBenchmarkBase
{
    private UserRepository _repo = null!;
    private readonly List<string> _seededUserIds = new();
    private string _knownUserId = string.Empty;
    private string _knownEmail = string.Empty;

    protected override async Task SetupAsync()
    {
        _repo = new UserRepository(Fixture.Context);

        for (int i = 0; i < 20; i++)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = $"bench-user-{i}@example.com",
                FullName = $"Benchmark User {i}",
                PasswordHash = "$2a$12$fakehashforperftesting00000000000000000000000000",
                Status = "active",
                Roles = i % 4 == 0 ? new List<string> { "admin" } : new List<string> { "author" },
                EmailVerified = true
            };

            await _repo.CreateAsync(user, TenantSlug);
            _seededUserIds.Add(user.Id);
        }

        _knownUserId = _seededUserIds[0];
        _knownEmail = "bench-user-0@example.com";
    }

    [Benchmark(Description = "User: Insert single document")]
    public async Task<User> CreateUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = $"perf-{Guid.NewGuid():N}@example.com",
            FullName = "Perf Test User",
            PasswordHash = "$2a$12$fakehashforperftesting00000000000000000000000000",
            Status = "active",
            Roles = new List<string> { "author" },
            EmailVerified = true
        };

        return await _repo.CreateAsync(user, TenantSlug);
    }

    [Benchmark(Description = "User: Get by ID (KV lookup)")]
    public async Task<User?> GetUserById()
    {
        return await _repo.GetByIdAsync(_knownUserId, TenantSlug);
    }

    [Benchmark(Description = "User: Get by email (N1QL)")]
    public async Task<User?> GetUserByEmail()
    {
        return await _repo.GetByEmailAsync(_knownEmail, TenantSlug);
    }

    [Benchmark(Description = "User: Update document")]
    public async Task<User> UpdateUser()
    {
        var user = (await _repo.GetByIdAsync(_knownUserId, TenantSlug))!;
        user.FullName = $"Updated at {DateTime.UtcNow:O}";
        return await _repo.UpdateAsync(user, TenantSlug);
    }

    [Benchmark(Description = "User: Batch get by IDs (N1QL IN)")]
    public async Task<Dictionary<string, User>> GetUsersByIds()
    {
        var ids = _seededUserIds.Take(10).ToList();
        return await _repo.GetByIdsAsync(ids, TenantSlug);
    }

    [Benchmark(Description = "User: Count by role (N1QL)")]
    public async Task<int> CountByRole()
    {
        return await _repo.CountByRoleAsync("admin", TenantSlug);
    }

    [Benchmark(Description = "User: List by role (N1QL)")]
    public async Task<List<User>> ListByRole()
    {
        return await _repo.ListByRoleAsync("author", TenantSlug);
    }
}
