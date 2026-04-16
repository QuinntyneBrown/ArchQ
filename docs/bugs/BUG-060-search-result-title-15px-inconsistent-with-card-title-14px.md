# BUG-060: Search result title 15px inconsistent with card title 14px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 40 - Search result consistency audit)

## Summary

`.search-result-title` uses `0.9375rem` (15px) but `.card-title` (fixed in BUG-052) now uses `0.875rem` (14px). Both display ADR titles in list contexts and should be consistent at 14px (`$font-sm`).

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.search-result-title`

## Recommended Fix

Change `.search-result-title` font-size from `0.9375rem` to `0.875rem`.
