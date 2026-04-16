# BUG-039: Brand logo row gap is 8px instead of design's 12px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 16 - Brand spacing audit)

## Summary

The gap between the logo icon and "ArchQ" text on auth pages is `0.5rem` (8px) but the design specifies `gap: 12` (12px). This makes the icon and text appear slightly closer together than intended.

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss` — `.brand` gap
- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss` — `.brand` gap

## Recommended Fix

Change `.brand` gap from `0.5rem` to `0.75rem` (12px).
