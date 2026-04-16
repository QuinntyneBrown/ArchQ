# BUG-074: Filters row margin-bottom 20px should be 24px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR List spacing)

## Summary

`.filters-row` has `margin-bottom: 1.25rem` (20px) but the design `Main Content` uses gap 24 between all children. The spacing between the filter row and the table should be 24px (`1.5rem`) to match the rest of the layout.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:53` — `.filters-row`

## Recommended Fix

Change `.filters-row` margin-bottom from `1.25rem` to `1.5rem`.
