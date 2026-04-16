# BUG-046: Author name color is #D1D5DB instead of $text-secondary #9CA3AF

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 23 - Table author text audit)

## Summary

The author name text in the ADR table uses `#D1D5DB` but the design specifies `#9CA3AF` (`$text-secondary`) for author names in table rows. This makes author names brighter than designed.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:225` — `.author-name`

## Recommended Fix

Change `.author-name` color from `#d1d5db` to `#9CA3AF`.
