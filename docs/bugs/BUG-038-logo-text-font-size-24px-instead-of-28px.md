# BUG-038: Logo "ArchQ" text is 24px instead of design's 28px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 15 - Logo typography audit)

## Summary

The "ArchQ" logo text on the Login and Register pages uses `font-size: 1.5rem` (24px) but the design specifies `fontSize: 28` for the logo text in both the Login and Register card components.

## Design vs Implementation

| Property | Design | Implementation |
|----------|--------|---------------|
| Logo text font-size | 28px | 24px (1.5rem) |

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss` — `.logo-text`
- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss` — `.logo-text`

## Recommended Fix

Change `.logo-text` font-size from `1.5rem` to `1.75rem` (28px).
