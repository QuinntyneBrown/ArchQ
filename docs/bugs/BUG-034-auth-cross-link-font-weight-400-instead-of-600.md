# BUG-034: Auth cross-link font-weight is 400 instead of 600

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 11 - Auth link audit)

## Summary

The "Sign up" link on the login page and the "Sign in" link on the register page use `font-weight: 400` (normal) but the design specifies `fontWeight: 600` (semibold) for these cross-navigation links. This makes the links less visually prominent than intended.

## Design vs Implementation

| Element | Design | Implementation |
|---------|--------|---------------|
| Login "Sign up" link | font-weight: 600 | font-weight: 400 |
| Register "Sign in" link | font-weight: 600 | font-weight: 400 |

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss` — `.sign-up-prompt a`
- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss` — `.sign-in-prompt a`

## Recommended Fix

Add `font-weight: 600` to the `.sign-up-prompt a` and `.sign-in-prompt a` rules.
