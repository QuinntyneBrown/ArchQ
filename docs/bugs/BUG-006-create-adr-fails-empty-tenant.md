# BUG-006: Creating first ADR in a new tenant throws 500 error

**Severity:** Critical  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 6)

## Summary

Creating the first ADR in a new tenant (organization) fails with a 500 Internal Server Error. The `AdrRepository.GetMaxAdrNumberAsync` method uses `MAX()` to find the highest ADR number, but when no ADRs exist, the query returns a JSON `null` value. The null check `is not null` passes because the Newtonsoft.Json `JToken` object itself is not null, but the subsequent cast `(long)row.maxNum` throws `ArgumentException: Can not convert Null to Int64`.

## Stack Trace

```
System.ArgumentException: Can not convert Null to Int64.
   at Newtonsoft.Json.Linq.JToken.op_Explicit(JToken value)
   at AdrRepository.GetMaxAdrNumberAsync(String tenantSlug) in AdrRepository.cs:line 250
   at CreateAdrCommandHandler.Handle(...)
```

## Affected File

- `src/ArchQ.Infrastructure/Persistence/Repositories/AdrRepository.cs:240-255`

## Root Cause

Line 248: `if (row.maxNum is not null)` — the `JToken` object is not C# null, but its JSON value is null. Line 250: `(int)(long)row.maxNum` fails because `JToken.op_Explicit(long)` cannot convert a null JSON value.

## Recommended Fix

Replace the null check with a proper JToken value check, or use a try-catch, or handle the null case before casting:
```csharp
var val = row.maxNum;
if (val is not null && val.Type != Newtonsoft.Json.Linq.JTokenType.Null)
    return (int)(long)val;
```
