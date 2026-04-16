# BUG-052: Mobile card title font-size is 15px instead of design's 14px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 29 - Card title size audit)

## Summary

The `.card-title` on mobile ADR cards uses `0.9375rem` (15px) but the design specifies `fontSize: 14` (`$font-sm`).

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.card-title`

## Recommended Fix

Change `.card-title` font-size from `0.9375rem` to `0.875rem` (14px).
