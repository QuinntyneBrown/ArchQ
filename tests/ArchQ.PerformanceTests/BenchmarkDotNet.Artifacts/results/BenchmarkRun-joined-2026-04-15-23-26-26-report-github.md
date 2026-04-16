```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK 11.0.100-preview.1.26104.118
  [Host]     : .NET 11.0.0 (11.0.26.10518), Arm64 RyuJIT AdvSIMD
  Job-MBTIOM : .NET 11.0.0 (11.0.26.10518), Arm64 RyuJIT AdvSIMD

IterationCount=10  WarmupCount=3  

```
| Type                  | Method                                    | SeedCount | DocumentCount | Mean | Error |
|---------------------- |------------------------------------------ |---------- |-------------- |-----:|------:|
| **AdrVersionBenchmarks**  | **&#39;AdrVersion: Insert version snapshot&#39;**     | **?**         | **?**             |   **NA** |    **NA** |
| AttachmentBenchmarks  | &#39;Attachment: Insert metadata&#39;             | ?         | ?             |   NA |    NA |
| AuditBenchmarks       | &#39;Audit: Write single entry&#39;               | ?         | ?             |   NA |    NA |
| CommentBenchmarks     | &#39;Comment: Insert single document&#39;         | ?         | ?             |   NA |    NA |
| GeneralNoteBenchmarks | &#39;GeneralNote: Insert single document&#39;     | ?         | ?             |   NA |    NA |
| MeetingNoteBenchmarks | &#39;MeetingNote: Insert single document&#39;     | ?         | ?             |   NA |    NA |
| TagBenchmarks         | &#39;Tag: Insert single document&#39;             | ?         | ?             |   NA |    NA |
| UserBenchmarks        | &#39;User: Insert single document&#39;            | ?         | ?             |   NA |    NA |
| AdrVersionBenchmarks  | &#39;AdrVersion: List by ADR (N1QL)&#39;          | ?         | ?             |   NA |    NA |
| AttachmentBenchmarks  | &#39;Attachment: Get by ID (KV lookup)&#39;       | ?         | ?             |   NA |    NA |
| AuditBenchmarks       | &#39;Audit: List with no filters&#39;             | ?         | ?             |   NA |    NA |
| CommentBenchmarks     | &#39;Comment: Get by ID (KV lookup)&#39;          | ?         | ?             |   NA |    NA |
| GeneralNoteBenchmarks | &#39;GeneralNote: Get by ID (KV lookup)&#39;      | ?         | ?             |   NA |    NA |
| MeetingNoteBenchmarks | &#39;MeetingNote: Get by ID (KV lookup)&#39;      | ?         | ?             |   NA |    NA |
| TagBenchmarks         | &#39;Tag: Get by slug (KV lookup)&#39;            | ?         | ?             |   NA |    NA |
| UserBenchmarks        | &#39;User: Get by ID (KV lookup)&#39;             | ?         | ?             |   NA |    NA |
| AdrVersionBenchmarks  | &#39;AdrVersion: Get specific version (N1QL)&#39; | ?         | ?             |   NA |    NA |
| AttachmentBenchmarks  | &#39;Attachment: List by ADR (N1QL)&#39;          | ?         | ?             |   NA |    NA |
| AuditBenchmarks       | &#39;Audit: List filtered by action&#39;          | ?         | ?             |   NA |    NA |
| CommentBenchmarks     | &#39;Comment: List by ADR (N1QL)&#39;             | ?         | ?             |   NA |    NA |
| GeneralNoteBenchmarks | &#39;GeneralNote: List by ADR (N1QL)&#39;         | ?         | ?             |   NA |    NA |
| MeetingNoteBenchmarks | &#39;MeetingNote: List by ADR (N1QL)&#39;         | ?         | ?             |   NA |    NA |
| TagBenchmarks         | &#39;Tag: List all (N1QL)&#39;                    | ?         | ?             |   NA |    NA |
| UserBenchmarks        | &#39;User: Get by email (N1QL)&#39;               | ?         | ?             |   NA |    NA |
| AuditBenchmarks       | &#39;Audit: List by resource ID&#39;              | ?         | ?             |   NA |    NA |
| CommentBenchmarks     | &#39;Comment: Update with CAS&#39;                | ?         | ?             |   NA |    NA |
| GeneralNoteBenchmarks | &#39;GeneralNote: Update with CAS&#39;            | ?         | ?             |   NA |    NA |
| MeetingNoteBenchmarks | &#39;MeetingNote: Update with CAS&#39;            | ?         | ?             |   NA |    NA |
| TagBenchmarks         | &#39;Tag: Search by prefix (N1QL LIKE)&#39;       | ?         | ?             |   NA |    NA |
| UserBenchmarks        | &#39;User: Update document&#39;                   | ?         | ?             |   NA |    NA |
| TagBenchmarks         | &#39;Tag: Increment usage count&#39;              | ?         | ?             |   NA |    NA |
| UserBenchmarks        | &#39;User: Batch get by IDs (N1QL IN)&#39;        | ?         | ?             |   NA |    NA |
| UserBenchmarks        | &#39;User: Count by role (N1QL)&#39;              | ?         | ?             |   NA |    NA |
| UserBenchmarks        | &#39;User: List by role (N1QL)&#39;               | ?         | ?             |   NA |    NA |
| **AdrBenchmarks**         | **&#39;ADR: Insert single document&#39;**             | **10**        | **?**             |   **NA** |    **NA** |
| AdrBenchmarks         | &#39;ADR: Get by ID (KV lookup)&#39;              | 10        | ?             |   NA |    NA |
| AdrBenchmarks         | &#39;ADR: Update with CAS&#39;                    | 10        | ?             |   NA |    NA |
| AdrBenchmarks         | &#39;ADR: List with N1QL (no filters)&#39;        | 10        | ?             |   NA |    NA |
| AdrBenchmarks         | &#39;ADR: List filtered by status&#39;            | 10        | ?             |   NA |    NA |
| AdrBenchmarks         | &#39;ADR: List filtered by tags&#39;              | 10        | ?             |   NA |    NA |
| AdrBenchmarks         | &#39;ADR: Count with filters&#39;                 | 10        | ?             |   NA |    NA |
| AdrBenchmarks         | &#39;ADR: Get max ADR number&#39;                 | 10        | ?             |   NA |    NA |
| **SearchBenchmarks**      | **&#39;Search: Single keyword&#39;**                  | **?**         | **20**            |   **NA** |    **NA** |
| SearchBenchmarks      | &#39;Search: Multi-word query&#39;                | ?         | 20            |   NA |    NA |
| SearchBenchmarks      | &#39;Search: With status filter&#39;              | ?         | 20            |   NA |    NA |
| SearchBenchmarks      | &#39;Search: Broad query (many matches)&#39;      | ?         | 20            |   NA |    NA |
| SearchBenchmarks      | &#39;Search: Narrow query (few matches)&#39;      | ?         | 20            |   NA |    NA |
| **AdrBenchmarks**         | **&#39;ADR: Insert single document&#39;**             | **50**        | **?**             |   **NA** |    **NA** |
| AdrBenchmarks         | &#39;ADR: Get by ID (KV lookup)&#39;              | 50        | ?             |   NA |    NA |
| AdrBenchmarks         | &#39;ADR: Update with CAS&#39;                    | 50        | ?             |   NA |    NA |
| AdrBenchmarks         | &#39;ADR: List with N1QL (no filters)&#39;        | 50        | ?             |   NA |    NA |
| AdrBenchmarks         | &#39;ADR: List filtered by status&#39;            | 50        | ?             |   NA |    NA |
| AdrBenchmarks         | &#39;ADR: List filtered by tags&#39;              | 50        | ?             |   NA |    NA |
| AdrBenchmarks         | &#39;ADR: Count with filters&#39;                 | 50        | ?             |   NA |    NA |
| AdrBenchmarks         | &#39;ADR: Get max ADR number&#39;                 | 50        | ?             |   NA |    NA |
| **SearchBenchmarks**      | **&#39;Search: Single keyword&#39;**                  | **?**         | **100**           |   **NA** |    **NA** |
| SearchBenchmarks      | &#39;Search: Multi-word query&#39;                | ?         | 100           |   NA |    NA |
| SearchBenchmarks      | &#39;Search: With status filter&#39;              | ?         | 100           |   NA |    NA |
| SearchBenchmarks      | &#39;Search: Broad query (many matches)&#39;      | ?         | 100           |   NA |    NA |
| SearchBenchmarks      | &#39;Search: Narrow query (few matches)&#39;      | ?         | 100           |   NA |    NA |

Benchmarks with issues:
  AdrVersionBenchmarks.'AdrVersion: Insert version snapshot': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  AttachmentBenchmarks.'Attachment: Insert metadata': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  AuditBenchmarks.'Audit: Write single entry': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  CommentBenchmarks.'Comment: Insert single document': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  GeneralNoteBenchmarks.'GeneralNote: Insert single document': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  MeetingNoteBenchmarks.'MeetingNote: Insert single document': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  TagBenchmarks.'Tag: Insert single document': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  UserBenchmarks.'User: Insert single document': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  AdrVersionBenchmarks.'AdrVersion: List by ADR (N1QL)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  AttachmentBenchmarks.'Attachment: Get by ID (KV lookup)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  AuditBenchmarks.'Audit: List with no filters': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  CommentBenchmarks.'Comment: Get by ID (KV lookup)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  GeneralNoteBenchmarks.'GeneralNote: Get by ID (KV lookup)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  MeetingNoteBenchmarks.'MeetingNote: Get by ID (KV lookup)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  TagBenchmarks.'Tag: Get by slug (KV lookup)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  UserBenchmarks.'User: Get by ID (KV lookup)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  AdrVersionBenchmarks.'AdrVersion: Get specific version (N1QL)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  AttachmentBenchmarks.'Attachment: List by ADR (N1QL)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  AuditBenchmarks.'Audit: List filtered by action': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  CommentBenchmarks.'Comment: List by ADR (N1QL)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  GeneralNoteBenchmarks.'GeneralNote: List by ADR (N1QL)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  MeetingNoteBenchmarks.'MeetingNote: List by ADR (N1QL)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  TagBenchmarks.'Tag: List all (N1QL)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  UserBenchmarks.'User: Get by email (N1QL)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  AuditBenchmarks.'Audit: List by resource ID': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  CommentBenchmarks.'Comment: Update with CAS': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  GeneralNoteBenchmarks.'GeneralNote: Update with CAS': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  MeetingNoteBenchmarks.'MeetingNote: Update with CAS': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  TagBenchmarks.'Tag: Search by prefix (N1QL LIKE)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  UserBenchmarks.'User: Update document': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  TagBenchmarks.'Tag: Increment usage count': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  UserBenchmarks.'User: Batch get by IDs (N1QL IN)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  UserBenchmarks.'User: Count by role (N1QL)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  UserBenchmarks.'User: List by role (N1QL)': Job-MBTIOM(IterationCount=10, WarmupCount=3)
  AdrBenchmarks.'ADR: Insert single document': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=10]
  AdrBenchmarks.'ADR: Get by ID (KV lookup)': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=10]
  AdrBenchmarks.'ADR: Update with CAS': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=10]
  AdrBenchmarks.'ADR: List with N1QL (no filters)': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=10]
  AdrBenchmarks.'ADR: List filtered by status': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=10]
  AdrBenchmarks.'ADR: List filtered by tags': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=10]
  AdrBenchmarks.'ADR: Count with filters': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=10]
  AdrBenchmarks.'ADR: Get max ADR number': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=10]
  SearchBenchmarks.'Search: Single keyword': Job-MBTIOM(IterationCount=10, WarmupCount=3) [DocumentCount=20]
  SearchBenchmarks.'Search: Multi-word query': Job-MBTIOM(IterationCount=10, WarmupCount=3) [DocumentCount=20]
  SearchBenchmarks.'Search: With status filter': Job-MBTIOM(IterationCount=10, WarmupCount=3) [DocumentCount=20]
  SearchBenchmarks.'Search: Broad query (many matches)': Job-MBTIOM(IterationCount=10, WarmupCount=3) [DocumentCount=20]
  SearchBenchmarks.'Search: Narrow query (few matches)': Job-MBTIOM(IterationCount=10, WarmupCount=3) [DocumentCount=20]
  AdrBenchmarks.'ADR: Insert single document': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=50]
  AdrBenchmarks.'ADR: Get by ID (KV lookup)': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=50]
  AdrBenchmarks.'ADR: Update with CAS': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=50]
  AdrBenchmarks.'ADR: List with N1QL (no filters)': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=50]
  AdrBenchmarks.'ADR: List filtered by status': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=50]
  AdrBenchmarks.'ADR: List filtered by tags': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=50]
  AdrBenchmarks.'ADR: Count with filters': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=50]
  AdrBenchmarks.'ADR: Get max ADR number': Job-MBTIOM(IterationCount=10, WarmupCount=3) [SeedCount=50]
  SearchBenchmarks.'Search: Single keyword': Job-MBTIOM(IterationCount=10, WarmupCount=3) [DocumentCount=100]
  SearchBenchmarks.'Search: Multi-word query': Job-MBTIOM(IterationCount=10, WarmupCount=3) [DocumentCount=100]
  SearchBenchmarks.'Search: With status filter': Job-MBTIOM(IterationCount=10, WarmupCount=3) [DocumentCount=100]
  SearchBenchmarks.'Search: Broad query (many matches)': Job-MBTIOM(IterationCount=10, WarmupCount=3) [DocumentCount=100]
  SearchBenchmarks.'Search: Narrow query (few matches)': Job-MBTIOM(IterationCount=10, WarmupCount=3) [DocumentCount=100]
