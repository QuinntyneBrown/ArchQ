# BUG-068: Author avatar wrong colors and size

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR List author avatars)

## Summary

`.author-avatar` has multiple discrepancies from the design Avatar component:
1. Background color: `#374151` (gray) — design specifies `#4F46E5` (indigo)
2. Text color: `#d1d5db` — design specifies `#FFFFFF` (white)
3. Size: `1.75rem` (28px) — design specifies 32px (`2rem`)
4. Font-size: `0.625rem` (10px) — design specifies 12px (`0.75rem`)

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:207` — `.author-avatar`

## Recommended Fix

Change `.author-avatar`:
- background-color from `#374151` to `#4F46E5`
- color from `#d1d5db` to `#FFFFFF`
- width/height from `1.75rem` to `2rem`
- font-size from `0.625rem` to `0.75rem`
