# BUG-064: Table row vertical padding 12px should be 14px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR List table rows)

## Summary

`.adr-table td` uses `0.75rem 1rem` (12px 16px) padding but the design specifies table row padding of [14,16] = 14px 16px (`0.875rem 1rem`). The vertical padding is 2px shorter than the design spec.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:140` — `.adr-table td`

## Recommended Fix

Change `.adr-table td` padding from `0.75rem 1rem` to `0.875rem 1rem`.
