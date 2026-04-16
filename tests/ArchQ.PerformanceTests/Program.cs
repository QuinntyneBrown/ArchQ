using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;
using ArchQ.PerformanceTests.Benchmarks;
using ArchQ.PerformanceTests.Infrastructure;

// ──────────────────────────────────────────────────────────────
// ArchQ Performance Tests — BenchmarkDotNet against real Couchbase
//
// Prerequisites:
//   1. Couchbase running locally (or set env vars below)
//   2. Bucket "archq" must exist
//
// Environment variables (optional overrides):
//   COUCHBASE_CONNECTION_STRING  (default: couchbase://localhost)
//   COUCHBASE_USERNAME           (default: Administrator)
//   COUCHBASE_PASSWORD           (default: password123)
//   COUCHBASE_BUCKET             (default: archq)
//
// Usage:
//   dotnet run -c Release                      # interactive picker
//   dotnet run -c Release -- --filter *Adr*    # run ADR benchmarks only
//   dotnet run -c Release -- --filter *Search* # run Search benchmarks only
//   dotnet run -c Release -- --list flat       # list all benchmarks
// ──────────────────────────────────────────────────────────────

var config = ManualConfig
    .CreateMinimumViable()
    .AddExporter(MarkdownExporter.GitHub)
    .WithOption(ConfigOptions.JoinSummary, true);

try
{
    BenchmarkSwitcher
        .FromTypes(
        [
            typeof(AdrBenchmarks),
            typeof(UserBenchmarks),
            typeof(CommentBenchmarks),
            typeof(TagBenchmarks),
            typeof(MeetingNoteBenchmarks),
            typeof(GeneralNoteBenchmarks),
            typeof(AttachmentBenchmarks),
            typeof(AdrVersionBenchmarks),
            typeof(AuditBenchmarks),
            typeof(SearchBenchmarks),
        ])
        .Run(args, config);
}
finally
{
    // Clean up the benchmark scope after all runs
    await CouchbaseFixture.Shared.DisposeAsync();
}
