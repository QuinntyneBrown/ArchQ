# BUG-049: Table date cell font-size is 13px instead of design's 12px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 26 - Table date font-size audit)

## Summary

The table date cell uses `font-size: 0.8125rem !important` (13px) but the design specifies `fontSize: 12` for the Modified date column text.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.date-cell`

## Recommended Fix

Change `.date-cell` font-size from `0.8125rem` to `0.75rem` (12px).
