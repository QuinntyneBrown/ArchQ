```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
unknown, 1 CPU, 16 logical and 16 physical cores
.NET SDK 8.0.126
  [Host]     : .NET 8.0.26 (8.0.2626.16921), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-DSEXMN : .NET 8.0.26 (8.0.2626.16921), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

IterationCount=10  WarmupCount=3  

```
| Type                  | Method                                    | SeedCount | DocumentCount | Mean           | Error         | StdDev        | Gen0      | Allocated  |
|---------------------- |------------------------------------------ |---------- |-------------- |---------------:|--------------:|--------------:|----------:|-----------:|
| **AdrVersionBenchmarks**  | **&#39;AdrVersion: Insert version snapshot&#39;**     | **?**         | **?**             |       **646.8 μs** |      **25.27 μs** |      **13.22 μs** |    **1.9531** |    **9.47 KB** |
| AttachmentBenchmarks  | &#39;Attachment: Insert metadata&#39;             | ?         | ?             |       741.1 μs |      21.29 μs |      14.08 μs |    2.9297 |    12.7 KB |
| AuditBenchmarks       | &#39;Audit: Write single entry&#39;               | ?         | ?             |       709.2 μs |      57.61 μs |      38.10 μs |    1.9531 |    9.56 KB |
| CommentBenchmarks     | &#39;Comment: Insert single document&#39;         | ?         | ?             |       790.0 μs |      71.63 μs |      47.38 μs |    2.9297 |   12.54 KB |
| GeneralNoteBenchmarks | &#39;GeneralNote: Insert single document&#39;     | ?         | ?             |       707.0 μs |      79.24 μs |      52.41 μs |    1.9531 |    9.45 KB |
| MeetingNoteBenchmarks | &#39;MeetingNote: Insert single document&#39;     | ?         | ?             |       718.5 μs |      60.65 μs |      40.12 μs |    1.9531 |   12.83 KB |
| TagBenchmarks         | &#39;Tag: Insert single document&#39;             | ?         | ?             |       614.4 μs |      31.84 μs |      21.06 μs |    1.9531 |     9.4 KB |
| UserBenchmarks        | &#39;User: Insert single document&#39;            | ?         | ?             |       740.0 μs |      54.40 μs |      35.98 μs |    1.9531 |   13.08 KB |
| AdrVersionBenchmarks  | &#39;AdrVersion: List by ADR (N1QL)&#39;          | ?         | ?             |   846,816.7 μs |  35,487.74 μs |  23,472.95 μs |         - |   38.66 KB |
| AttachmentBenchmarks  | &#39;Attachment: Get by ID (KV lookup)&#39;       | ?         | ?             |       532.8 μs |      46.04 μs |      30.45 μs |    1.9531 |    7.98 KB |
| AuditBenchmarks       | &#39;Audit: List with no filters&#39;             | ?         | ?             |   922,728.0 μs |  35,179.72 μs |  20,934.89 μs |         - |   69.45 KB |
| CommentBenchmarks     | &#39;Comment: Get by ID (KV lookup)&#39;          | ?         | ?             |       490.3 μs |      85.39 μs |      56.48 μs |    0.9766 |    7.76 KB |
| GeneralNoteBenchmarks | &#39;GeneralNote: Get by ID (KV lookup)&#39;      | ?         | ?             |       405.4 μs |      46.27 μs |      30.60 μs |    0.9766 |    7.73 KB |
| MeetingNoteBenchmarks | &#39;MeetingNote: Get by ID (KV lookup)&#39;      | ?         | ?             |       455.0 μs |      46.57 μs |      30.80 μs |    1.9531 |    8.19 KB |
| TagBenchmarks         | &#39;Tag: Get by slug (KV lookup)&#39;            | ?         | ?             |             NA |            NA |            NA |        NA |         NA |
| UserBenchmarks        | &#39;User: Get by ID (KV lookup)&#39;             | ?         | ?             |       482.0 μs |      41.05 μs |      27.15 μs |    1.9531 |    8.07 KB |
| AdrVersionBenchmarks  | &#39;AdrVersion: Get specific version (N1QL)&#39; | ?         | ?             |   925,418.5 μs | 100,866.64 μs |  66,717.04 μs |         - |   26.84 KB |
| AttachmentBenchmarks  | &#39;Attachment: List by ADR (N1QL)&#39;          | ?         | ?             |   855,097.9 μs |  29,522.21 μs |  19,527.12 μs |         - |   38.69 KB |
| AuditBenchmarks       | &#39;Audit: List filtered by action&#39;          | ?         | ?             |   920,670.3 μs |  45,967.30 μs |  24,041.79 μs |         - |   64.11 KB |
| CommentBenchmarks     | &#39;Comment: List by ADR (N1QL)&#39;             | ?         | ?             |     5,104.5 μs |     500.50 μs |     261.77 μs |    7.8125 |   38.91 KB |
| GeneralNoteBenchmarks | &#39;GeneralNote: List by ADR (N1QL)&#39;         | ?         | ?             |   862,915.7 μs |  41,143.12 μs |  24,483.61 μs |         - |   36.55 KB |
| MeetingNoteBenchmarks | &#39;MeetingNote: List by ADR (N1QL)&#39;         | ?         | ?             |   432,974.0 μs |  14,231.32 μs |   9,413.14 μs |         - |   40.35 KB |
| TagBenchmarks         | &#39;Tag: List all (N1QL)&#39;                    | ?         | ?             |             NA |            NA |            NA |        NA |         NA |
| UserBenchmarks        | &#39;User: Get by email (N1QL)&#39;               | ?         | ?             |     2,846.7 μs |     206.33 μs |     136.47 μs |    3.9063 |   22.78 KB |
| AuditBenchmarks       | &#39;Audit: List by resource ID&#39;              | ?         | ?             |   474,331.3 μs |  47,122.58 μs |  31,168.67 μs |         - |   34.86 KB |
| CommentBenchmarks     | &#39;Comment: Update with CAS&#39;                | ?         | ?             |     1,444.6 μs |      88.37 μs |      52.59 μs |    3.9063 |   25.92 KB |
| GeneralNoteBenchmarks | &#39;GeneralNote: Update with CAS&#39;            | ?         | ?             |     1,401.6 μs |      67.20 μs |      44.45 μs |    3.9063 |   22.88 KB |
| MeetingNoteBenchmarks | &#39;MeetingNote: Update with CAS&#39;            | ?         | ?             |     1,588.7 μs |     170.63 μs |     112.86 μs |    5.8594 |   26.55 KB |
| TagBenchmarks         | &#39;Tag: Search by prefix (N1QL LIKE)&#39;       | ?         | ?             |             NA |            NA |            NA |        NA |         NA |
| UserBenchmarks        | &#39;User: Update document&#39;                   | ?         | ?             |     1,161.4 μs |      93.74 μs |      62.00 μs |    3.9063 |   20.61 KB |
| TagBenchmarks         | &#39;Tag: Increment usage count&#39;              | ?         | ?             |             NA |            NA |            NA |        NA |         NA |
| UserBenchmarks        | &#39;User: Batch get by IDs (N1QL IN)&#39;        | ?         | ?             |   449,865.8 μs |  54,909.25 μs |  36,319.07 μs |         - |   42.93 KB |
| UserBenchmarks        | &#39;User: Count by role (N1QL)&#39;              | ?         | ?             |   486,758.5 μs |  39,183.08 μs |  25,917.18 μs |         - |   25.14 KB |
| UserBenchmarks        | &#39;User: List by role (N1QL)&#39;               | ?         | ?             |   489,244.4 μs |  40,782.78 μs |  26,975.29 μs | 1000.0000 | 9182.05 KB |
| **AdrBenchmarks**         | **&#39;ADR: Insert single document&#39;**             | **10**        | **?**             |       **619.7 μs** |      **21.89 μs** |      **11.45 μs** |    **2.9297** |    **12.9 KB** |
| AdrBenchmarks         | &#39;ADR: Get by ID (KV lookup)&#39;              | 10        | ?             |       371.8 μs |      35.94 μs |      23.77 μs |    1.9531 |    8.22 KB |
| AdrBenchmarks         | &#39;ADR: Update with CAS&#39;                    | 10        | ?             |     1,355.4 μs |      69.50 μs |      45.97 μs |    5.8594 |   26.67 KB |
| AdrBenchmarks         | &#39;ADR: List with N1QL (no filters)&#39;        | 10        | ?             |   929,418.5 μs |  41,493.74 μs |  24,692.26 μs |         - |   51.95 KB |
| AdrBenchmarks         | &#39;ADR: List filtered by status&#39;            | 10        | ?             |   912,389.9 μs |  23,035.03 μs |  12,047.77 μs |         - |   51.62 KB |
| AdrBenchmarks         | &#39;ADR: List filtered by tags&#39;              | 10        | ?             |   940,804.1 μs |  34,318.45 μs |  20,422.36 μs |         - |   55.66 KB |
| AdrBenchmarks         | &#39;ADR: Count with filters&#39;                 | 10        | ?             |   893,412.0 μs |  32,481.25 μs |  19,329.07 μs |         - |    26.7 KB |
| AdrBenchmarks         | &#39;ADR: Get max ADR number (N1QL)&#39;          | 10        | ?             |   882,012.6 μs |  18,903.45 μs |  12,503.46 μs |         - |   25.52 KB |
| **SearchBenchmarks**      | **&#39;Search: Single keyword&#39;**                  | **?**         | **20**            | **3,974,355.8 μs** | **102,072.36 μs** |  **67,514.54 μs** |         **-** |   **75.24 KB** |
| SearchBenchmarks      | &#39;Search: Multi-word query&#39;                | ?         | 20            | 4,288,227.4 μs | 189,196.02 μs | 125,141.45 μs |         - |   72.07 KB |
| SearchBenchmarks      | &#39;Search: With status filter&#39;              | ?         | 20            |    39,509.3 μs |   4,227.68 μs |   2,796.35 μs |         - |  146.43 KB |
| SearchBenchmarks      | &#39;Search: Broad query (many matches)&#39;      | ?         | 20            | 4,173,300.9 μs | 133,942.07 μs |  88,594.39 μs |         - |  169.86 KB |
| SearchBenchmarks      | &#39;Search: Narrow query (few matches)&#39;      | ?         | 20            | 4,039,727.5 μs | 156,897.20 μs | 103,777.78 μs |         - |  103.13 KB |
| **AdrBenchmarks**         | **&#39;ADR: Insert single document&#39;**             | **50**        | **?**             |       **617.3 μs** |      **33.27 μs** |      **22.00 μs** |    **2.9297** |    **12.9 KB** |
| AdrBenchmarks         | &#39;ADR: Get by ID (KV lookup)&#39;              | 50        | ?             |       363.1 μs |      31.43 μs |      20.79 μs |    1.9531 |    8.22 KB |
| AdrBenchmarks         | &#39;ADR: Update with CAS&#39;                    | 50        | ?             |     1,338.0 μs |      51.70 μs |      34.20 μs |    5.8594 |   26.68 KB |
| AdrBenchmarks         | &#39;ADR: List with N1QL (no filters)&#39;        | 50        | ?             | 2,092,684.3 μs | 214,945.28 μs | 142,172.99 μs |         - |    56.8 KB |
| AdrBenchmarks         | &#39;ADR: List filtered by status&#39;            | 50        | ?             | 2,148,930.1 μs | 173,636.82 μs | 114,850.00 μs |         - |   57.59 KB |
| AdrBenchmarks         | &#39;ADR: List filtered by tags&#39;              | 50        | ?             | 2,292,550.6 μs | 124,357.62 μs |  82,254.87 μs |         - |   60.52 KB |
| AdrBenchmarks         | &#39;ADR: Count with filters&#39;                 | 50        | ?             | 1,899,853.3 μs |  61,214.76 μs |  32,016.50 μs |         - |   30.98 KB |
| AdrBenchmarks         | &#39;ADR: Get max ADR number (N1QL)&#39;          | 50        | ?             | 1,994,281.3 μs | 183,176.98 μs | 121,160.23 μs |         - |   29.59 KB |
| **SearchBenchmarks**      | **&#39;Search: Single keyword&#39;**                  | **?**         | **100**           | **3,939,647.1 μs** |  **84,892.26 μs** |  **56,150.98 μs** |         **-** |  **155.72 KB** |
| SearchBenchmarks      | &#39;Search: Multi-word query&#39;                | ?         | 100           | 3,992,664.0 μs | 108,878.92 μs |  72,016.66 μs |         - |   63.84 KB |
| SearchBenchmarks      | &#39;Search: With status filter&#39;              | ?         | 100           |    53,435.7 μs |   4,593.16 μs |   3,038.09 μs |         - |  149.22 KB |
| SearchBenchmarks      | &#39;Search: Broad query (many matches)&#39;      | ?         | 100           | 3,999,244.3 μs | 143,429.70 μs |  94,869.86 μs |         - |  177.52 KB |
| SearchBenchmarks      | &#39;Search: Narrow query (few matches)&#39;      | ?         | 100           | 3,859,891.5 μs | 129,860.99 μs |  85,895.01 μs |         - |   166.3 KB |

Benchmarks with issues:
  TagBenchmarks.'Tag: Get by slug (KV lookup)': Job-DSEXMN(IterationCount=10, WarmupCount=3)
  TagBenchmarks.'Tag: List all (N1QL)': Job-DSEXMN(IterationCount=10, WarmupCount=3)
  TagBenchmarks.'Tag: Search by prefix (N1QL LIKE)': Job-DSEXMN(IterationCount=10, WarmupCount=3)
  TagBenchmarks.'Tag: Increment usage count': Job-DSEXMN(IterationCount=10, WarmupCount=3)
