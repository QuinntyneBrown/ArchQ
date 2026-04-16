# BUG-073: Tablet toolbar horizontal padding 16px should be 20px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - Tablet header toolbar)

## Summary

`.app-toolbar` uses `padding: 0.75rem 1rem` (12px 16px) but the design tablet header specifies `padding: [12,20]` (12px 20px). The horizontal padding is 4px less than the design spec.

## Affected File

- `src/ArchQ.Web/src/app/app.component.scss:173` — `.app-toolbar` inside `@media (max-width: 1024px)`

## Recommended Fix

Change `.app-toolbar` padding from `0.75rem 1rem` to `0.75rem 1.25rem`.
