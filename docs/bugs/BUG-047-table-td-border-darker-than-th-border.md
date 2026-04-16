# BUG-047: Table data row border color darker than header border

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 22 — table border audit)

## Summary

The ADR table header (`th`) border-bottom uses `#2E3142` (the correct `$border-default` token) but the data cells (`td`) use `#1F2231` — a darker shade. This creates an inconsistency where data row borders are less visible than the header border.

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:143`

## Recommended Fix

Change `.adr-table td { border-bottom: 1px solid #1F2231; }` to `#2E3142`.
