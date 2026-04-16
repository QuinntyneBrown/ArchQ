# BUG-023: Status filter doesn't match due to case sensitivity

**Severity:** High  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 25)

## Summary

Selecting "Draft" from the status filter dropdown shows 0 results even when all ADRs are in draft status. The N1QL query does a case-sensitive `a.status = $status` comparison, but the dropdown sends "Draft" (capitalized) while the database stores "draft" (lowercase).

## Root Cause

`AdrRepository.BuildFilterClauses()` line 192-193 passes the status parameter as-is without normalizing case. The ADR entity stores status as lowercase ("draft", "in review") but the Angular dropdown option values are capitalized ("Draft", "In Review").

## Affected File

- `src/ArchQ.Infrastructure/Persistence/Repositories/AdrRepository.cs:190-194`

## Recommended Fix

Lowercase the status parameter: `queryOptions.Parameter("status", listParams.Status.ToLowerInvariant())`.
