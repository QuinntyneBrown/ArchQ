# BUG-037: Auth page logo icon is 32px instead of design's 28px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 13 — logo audit)

## Summary

The ArchQ logo icon on both the login and register pages is `32px` (2rem) but the design specifies `28px`. This makes the icon slightly oversized relative to the 28px logo text.

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss:23-26`
- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss:23-26`

## Recommended Fix

Change `.logo-icon { width: 2rem; height: 2rem; }` to `{ width: 1.75rem; height: 1.75rem; }` (28px).
