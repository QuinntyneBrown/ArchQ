# BUG-059: Pagination button text color is #D1D5DB instead of $text-primary

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 38 - Pagination audit)

## Summary

The `.btn-page` pagination buttons use `#d1d5db` for text color. This non-token color should be `$text-primary` (`#F0F1F5`) for consistency.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:299` — `.btn-page`

## Recommended Fix

Change `.btn-page` color from `#d1d5db` to `#F0F1F5`.
