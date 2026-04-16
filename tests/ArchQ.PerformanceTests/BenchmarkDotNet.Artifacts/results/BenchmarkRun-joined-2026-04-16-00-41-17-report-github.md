```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK 11.0.100-preview.1.26104.118
  [Host]     : .NET 11.0.0 (11.0.26.10518), Arm64 RyuJIT AdvSIMD
  Job-IQQLNC : .NET 11.0.0 (11.0.26.10518), Arm64 RyuJIT AdvSIMD

IterationCount=10  WarmupCount=3  

```
| Type                  | Method                                    | SeedCount | DocumentCount | Mean        | Error        | StdDev       | Median      | Gen0   | Gen1   | Gen2   | Allocated |
|---------------------- |------------------------------------------ |---------- |-------------- |------------:|-------------:|-------------:|------------:|-------:|-------:|-------:|----------:|
| **AdrVersionBenchmarks**  | **&#39;AdrVersion: Insert version snapshot&#39;**     | **?**         | **?**             |          **NA** |           **NA** |           **NA** |          **NA** |     **NA** |     **NA** |     **NA** |        **NA** |
| AttachmentBenchmarks  | &#39;Attachment: Insert metadata&#39;             | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| AuditBenchmarks       | &#39;Audit: Write single entry&#39;               | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| CommentBenchmarks     | &#39;Comment: Insert single document&#39;         | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| GeneralNoteBenchmarks | &#39;GeneralNote: Insert single document&#39;     | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| MeetingNoteBenchmarks | &#39;MeetingNote: Insert single document&#39;     | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| TagBenchmarks         | &#39;Tag: Insert single document&#39;             | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| UserBenchmarks        | &#39;User: Insert single document&#39;            | ?         | ?             |   400.52 μs |   405.354 μs |   268.117 μs |   261.44 μs | 3.1738 |      - |      - |  13.12 KB |
| AdrVersionBenchmarks  | &#39;AdrVersion: List by ADR (N1QL)&#39;          | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| AttachmentBenchmarks  | &#39;Attachment: Get by ID (KV lookup)&#39;       | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| AuditBenchmarks       | &#39;Audit: List with no filters&#39;             | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| CommentBenchmarks     | &#39;Comment: Get by ID (KV lookup)&#39;          | ?         | ?             |    62.58 μs |     7.299 μs |     4.828 μs |    63.08 μs | 1.8311 | 0.7324 | 0.2441 |   7.88 KB |
| GeneralNoteBenchmarks | &#39;GeneralNote: Get by ID (KV lookup)&#39;      | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| MeetingNoteBenchmarks | &#39;MeetingNote: Get by ID (KV lookup)&#39;      | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| TagBenchmarks         | &#39;Tag: Get by slug (KV lookup)&#39;            | ?         | ?             |    64.38 μs |    10.548 μs |     6.977 μs |    65.85 μs | 1.7090 | 0.7324 | 0.1221 |    7.3 KB |
| UserBenchmarks        | &#39;User: Get by ID (KV lookup)&#39;             | ?         | ?             |    60.64 μs |     5.171 μs |     3.077 μs |    61.69 μs | 1.9531 | 0.7324 | 0.2441 |   8.32 KB |
| AdrVersionBenchmarks  | &#39;AdrVersion: Get specific version (N1QL)&#39; | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| AttachmentBenchmarks  | &#39;Attachment: List by ADR (N1QL)&#39;          | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| AuditBenchmarks       | &#39;Audit: List filtered by action&#39;          | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| CommentBenchmarks     | &#39;Comment: List by ADR (N1QL)&#39;             | ?         | ?             | 7,762.10 μs | 1,693.543 μs | 1,120.174 μs | 7,516.91 μs |      - |      - |      - |  39.91 KB |
| GeneralNoteBenchmarks | &#39;GeneralNote: List by ADR (N1QL)&#39;         | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| MeetingNoteBenchmarks | &#39;MeetingNote: List by ADR (N1QL)&#39;         | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| TagBenchmarks         | &#39;Tag: List all (N1QL)&#39;                    | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| UserBenchmarks        | &#39;User: Get by email (N1QL)&#39;               | ?         | ?             | 1,854.44 μs |   265.097 μs |   138.651 μs | 1,853.40 μs | 3.9063 |      - |      - |  23.57 KB |
| AuditBenchmarks       | &#39;Audit: List by resource ID&#39;              | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| CommentBenchmarks     | &#39;Comment: Update with CAS&#39;                | ?         | ?             |   222.15 μs |    12.388 μs |     7.372 μs |   219.47 μs | 6.3477 | 0.9766 |      - |  25.99 KB |
| GeneralNoteBenchmarks | &#39;GeneralNote: Update with CAS&#39;            | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| MeetingNoteBenchmarks | &#39;MeetingNote: Update with CAS&#39;            | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| TagBenchmarks         | &#39;Tag: Search by prefix (N1QL LIKE)&#39;       | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| UserBenchmarks        | &#39;User: Update document&#39;                   | ?         | ?             |   184.75 μs |     3.679 μs |     2.190 μs |   184.70 μs | 5.1270 | 0.2441 |      - |  20.58 KB |
| TagBenchmarks         | &#39;Tag: Increment usage count&#39;              | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| UserBenchmarks        | &#39;User: Batch get by IDs (N1QL IN)&#39;        | ?         | ?             | 2,810.89 μs |   245.702 μs |   146.213 μs | 2,793.75 μs |      - |      - |      - |  40.47 KB |
| UserBenchmarks        | &#39;User: Count by role (N1QL)&#39;              | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| UserBenchmarks        | &#39;User: List by role (N1QL)&#39;               | ?         | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| **AdrBenchmarks**         | **&#39;ADR: Insert single document&#39;**             | **10**        | **?**             |   **126.06 μs** |     **8.134 μs** |     **4.841 μs** |   **124.39 μs** | **3.1738** |      **-** |      **-** |  **13.01 KB** |
| AdrBenchmarks         | &#39;ADR: Get by ID (KV lookup)&#39;              | 10        | ?             |    77.92 μs |     2.696 μs |     1.783 μs |    77.85 μs | 2.0752 | 0.3662 |      - |   8.58 KB |
| AdrBenchmarks         | &#39;ADR: Update with CAS&#39;                    | 10        | ?             |   271.85 μs |    12.958 μs |     7.711 μs |   270.84 μs | 6.3477 | 0.9766 |      - |   26.8 KB |
| AdrBenchmarks         | &#39;ADR: List with N1QL (no filters)&#39;        | 10        | ?             | 2,650.51 μs |   392.057 μs |   233.307 μs | 2,635.83 μs | 7.8125 |      - |      - |  33.71 KB |
| AdrBenchmarks         | &#39;ADR: List filtered by status&#39;            | 10        | ?             | 3,722.13 μs | 1,060.294 μs |   630.964 μs | 3,695.17 μs |      - |      - |      - |  29.44 KB |
| AdrBenchmarks         | &#39;ADR: List filtered by tags&#39;              | 10        | ?             | 3,024.56 μs |   320.947 μs |   190.990 μs | 2,973.88 μs | 7.8125 |      - |      - |   37.6 KB |
| AdrBenchmarks         | &#39;ADR: Count with filters&#39;                 | 10        | ?             | 1,668.81 μs |   152.308 μs |    90.636 μs | 1,650.24 μs | 5.8594 |      - |      - |  23.92 KB |
| AdrBenchmarks         | &#39;ADR: Get max ADR number&#39;                 | 10        | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| **SearchBenchmarks**      | **&#39;Search: Single keyword&#39;**                  | **?**         | **20**            |          **NA** |           **NA** |           **NA** |          **NA** |     **NA** |     **NA** |     **NA** |        **NA** |
| SearchBenchmarks      | &#39;Search: Multi-word query&#39;                | ?         | 20            |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| SearchBenchmarks      | &#39;Search: With status filter&#39;              | ?         | 20            |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| SearchBenchmarks      | &#39;Search: Broad query (many matches)&#39;      | ?         | 20            |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| SearchBenchmarks      | &#39;Search: Narrow query (few matches)&#39;      | ?         | 20            |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| **AdrBenchmarks**         | **&#39;ADR: Insert single document&#39;**             | **50**        | **?**             |   **125.37 μs** |    **12.424 μs** |     **7.394 μs** |   **123.81 μs** | **3.1738** |      **-** |      **-** |  **12.88 KB** |
| AdrBenchmarks         | &#39;ADR: Get by ID (KV lookup)&#39;              | 50        | ?             |    74.15 μs |     6.102 μs |     3.631 μs |    72.84 μs | 2.0752 | 0.4883 | 0.2441 |   8.52 KB |
| AdrBenchmarks         | &#39;ADR: Update with CAS&#39;                    | 50        | ?             |   237.42 μs |     7.276 μs |     4.330 μs |   236.77 μs | 6.3477 | 1.4648 |      - |  26.77 KB |
| AdrBenchmarks         | &#39;ADR: List with N1QL (no filters)&#39;        | 50        | ?             | 5,617.47 μs | 1,419.588 μs |   844.774 μs | 5,398.81 μs |      - |      - |      - |  49.01 KB |
| AdrBenchmarks         | &#39;ADR: List filtered by status&#39;            | 50        | ?             | 4,091.35 μs |   682.096 μs |   356.749 μs | 4,115.12 μs | 7.8125 |      - |      - |  40.64 KB |
| AdrBenchmarks         | &#39;ADR: List filtered by tags&#39;              | 50        | ?             | 9,095.61 μs | 2,855.782 μs | 1,888.923 μs | 8,987.85 μs |      - |      - |      - |  56.21 KB |
| AdrBenchmarks         | &#39;ADR: Count with filters&#39;                 | 50        | ?             | 3,110.87 μs |   556.087 μs |   330.919 μs | 2,999.31 μs | 3.9063 |      - |      - |  24.02 KB |
| AdrBenchmarks         | &#39;ADR: Get max ADR number&#39;                 | 50        | ?             |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| **SearchBenchmarks**      | **&#39;Search: Single keyword&#39;**                  | **?**         | **100**           |          **NA** |           **NA** |           **NA** |          **NA** |     **NA** |     **NA** |     **NA** |        **NA** |
| SearchBenchmarks      | &#39;Search: Multi-word query&#39;                | ?         | 100           |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| SearchBenchmarks      | &#39;Search: With status filter&#39;              | ?         | 100           |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| SearchBenchmarks      | &#39;Search: Broad query (many matches)&#39;      | ?         | 100           |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |
| SearchBenchmarks      | &#39;Search: Narrow query (few matches)&#39;      | ?         | 100           |          NA |           NA |           NA |          NA |     NA |     NA |     NA |        NA |

Benchmarks with issues:
  AdrVersionBenchmarks.'AdrVersion: Insert version snapshot': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  AttachmentBenchmarks.'Attachment: Insert metadata': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  AuditBenchmarks.'Audit: Write single entry': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  CommentBenchmarks.'Comment: Insert single document': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  GeneralNoteBenchmarks.'GeneralNote: Insert single document': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  MeetingNoteBenchmarks.'MeetingNote: Insert single document': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  TagBenchmarks.'Tag: Insert single document': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  AdrVersionBenchmarks.'AdrVersion: List by ADR (N1QL)': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  AttachmentBenchmarks.'Attachment: Get by ID (KV lookup)': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  AuditBenchmarks.'Audit: List with no filters': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  GeneralNoteBenchmarks.'GeneralNote: Get by ID (KV lookup)': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  MeetingNoteBenchmarks.'MeetingNote: Get by ID (KV lookup)': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  AdrVersionBenchmarks.'AdrVersion: Get specific version (N1QL)': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  AttachmentBenchmarks.'Attachment: List by ADR (N1QL)': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  AuditBenchmarks.'Audit: List filtered by action': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  GeneralNoteBenchmarks.'GeneralNote: List by ADR (N1QL)': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  MeetingNoteBenchmarks.'MeetingNote: List by ADR (N1QL)': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  TagBenchmarks.'Tag: List all (N1QL)': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  AuditBenchmarks.'Audit: List by resource ID': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  GeneralNoteBenchmarks.'GeneralNote: Update with CAS': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  MeetingNoteBenchmarks.'MeetingNote: Update with CAS': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  TagBenchmarks.'Tag: Search by prefix (N1QL LIKE)': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  TagBenchmarks.'Tag: Increment usage count': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  UserBenchmarks.'User: Count by role (N1QL)': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  UserBenchmarks.'User: List by role (N1QL)': Job-IQQLNC(IterationCount=10, WarmupCount=3)
  AdrBenchmarks.'ADR: Get max ADR number': Job-IQQLNC(IterationCount=10, WarmupCount=3) [SeedCount=10]
  SearchBenchmarks.'Search: Single keyword': Job-IQQLNC(IterationCount=10, WarmupCount=3) [DocumentCount=20]
  SearchBenchmarks.'Search: Multi-word query': Job-IQQLNC(IterationCount=10, WarmupCount=3) [DocumentCount=20]
  SearchBenchmarks.'Search: With status filter': Job-IQQLNC(IterationCount=10, WarmupCount=3) [DocumentCount=20]
  SearchBenchmarks.'Search: Broad query (many matches)': Job-IQQLNC(IterationCount=10, WarmupCount=3) [DocumentCount=20]
  SearchBenchmarks.'Search: Narrow query (few matches)': Job-IQQLNC(IterationCount=10, WarmupCount=3) [DocumentCount=20]
  AdrBenchmarks.'ADR: Get max ADR number': Job-IQQLNC(IterationCount=10, WarmupCount=3) [SeedCount=50]
  SearchBenchmarks.'Search: Single keyword': Job-IQQLNC(IterationCount=10, WarmupCount=3) [DocumentCount=100]
  SearchBenchmarks.'Search: Multi-word query': Job-IQQLNC(IterationCount=10, WarmupCount=3) [DocumentCount=100]
  SearchBenchmarks.'Search: With status filter': Job-IQQLNC(IterationCount=10, WarmupCount=3) [DocumentCount=100]
  SearchBenchmarks.'Search: Broad query (many matches)': Job-IQQLNC(IterationCount=10, WarmupCount=3) [DocumentCount=100]
  SearchBenchmarks.'Search: Narrow query (few matches)': Job-IQQLNC(IterationCount=10, WarmupCount=3) [DocumentCount=100]
