# BUG-041: Login form group spacing is 16px instead of design's 20px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 18 - Form spacing audit)

## Summary

The gap between form fields (Email Address and Password) on the Login page is `16px` (`margin-bottom: 1rem`) but the design specifies `gap: 20` (20px) for the Login form group. Note: the Register form uses `gap: 16` in the design, which is correct as-is.

## Design vs Implementation

| Page | Design Gap | Implementation |
|------|-----------|---------------|
| Login | 20px | 16px (1rem) |
| Register | 16px | 16px (1rem) - correct |

## Affected File

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss` — `.form-group` margin-bottom

## Recommended Fix

Change `.form-group` margin-bottom from `1rem` to `1.25rem` (20px) in the login component SCSS.
