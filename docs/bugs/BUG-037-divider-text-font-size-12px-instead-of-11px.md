# BUG-037: Login divider "OR" text is 12px instead of design's 11px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 14 - Divider text audit)

## Summary

The "OR" divider text on the login page renders at `12px` (`0.75rem`) but the design specifies `11px` for this element. The design also uses `Geist Mono` font-family for the divider text, but the implementation inherits `Inter`.

## Design vs Implementation

| Property | Design | Implementation |
|----------|--------|---------------|
| Font-size | 11px | 12px (0.75rem) |
| Font-family | Geist Mono | Inter (inherited) |

## Affected File

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss:119-124` — `.divider-text`

## Recommended Fix

Change `.divider-text` font-size from `0.75rem` to `0.6875rem` (11px).
