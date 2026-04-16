# BUG-069: Search input vertical padding 8px should be 10px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR List search input)

## Summary

`.search-input` has vertical padding of `0.5rem` (8px) but the design Input/Search component specifies 10px (`0.625rem`) vertical padding. This makes the search input slightly shorter than intended.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:69` — `.search-input`

## Recommended Fix

Change `.search-input` padding from `0.5rem 0.75rem 0.5rem 2.25rem` to `0.625rem 0.75rem 0.625rem 2.25rem`.
