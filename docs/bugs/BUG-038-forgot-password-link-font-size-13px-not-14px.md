# BUG-038: Forgot password link font-size is 13px instead of design's 14px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 14 — typography audit)

## Summary

The "Forgot password?" link on the login page uses `font-size: 0.8125rem` (13px) but the design spec specifies 14px. This creates a subtle inconsistency with other 14px text on the page.

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss` — `.forgot-password`

## Recommended Fix

Change `font-size: 0.8125rem` to `font-size: 0.875rem` (14px).
