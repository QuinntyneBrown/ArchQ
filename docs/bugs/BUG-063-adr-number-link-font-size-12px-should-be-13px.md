# BUG-063: ADR number link font-size 12px should be 13px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR List table)

## Summary

`.adr-number-link` uses `0.75rem` (12px) font-size but the design specifies 13px (`0.8125rem`) for ADR number text in the table. This makes the number links slightly smaller than intended.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:157` — `.adr-number-link`

## Recommended Fix

Change `.adr-number-link` font-size from `0.75rem` to `0.8125rem`.
