# BUG-051: Table cell default color is #D1D5DB instead of $text-primary #F0F1F5

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 28 - Table cell color audit)

## Summary

The `.adr-table td` base color is `#d1d5db` which doesn't match any design token. The design shows table row title text at `#F0F1F5` (`$text-primary`). Since `.title-cell` doesn't have its own color override, it inherits the wrong base td color. Changing td to `$text-primary` aligns the title cell with the design while other columns (author, date) already have their own color overrides.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:142` — `.adr-table td`

## Recommended Fix

Change `.adr-table td` color from `#d1d5db` to `#F0F1F5`.
