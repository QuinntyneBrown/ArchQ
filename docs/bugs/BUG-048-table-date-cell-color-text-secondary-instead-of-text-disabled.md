# BUG-048: Table date cell color uses $text-secondary instead of $text-disabled

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 25 - Table date cell audit)

## Summary

The table date cell uses `#9CA3AF` (`$text-secondary`) with `!important` but the design specifies `#5C5F6E` (`$text-disabled`) for the "Modified" date column. Dates should be the dimmest text in the table to reduce visual clutter.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.date-cell`

## Recommended Fix

Change `.date-cell` color from `#9ca3af !important` to `#5C5F6E`.
