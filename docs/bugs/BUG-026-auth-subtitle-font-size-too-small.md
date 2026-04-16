# BUG-026: Auth page subtitle font size is 14px instead of 16px

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 3 - Register/Login typography audit)

## Summary

The subtitle text on both the Login and Register pages ("Sign in to your account" / "Create your account") renders at 14px (`$font-sm`) instead of the design-specified 16px (`$font-base`). This makes the subtitle appear smaller than intended and inconsistent with the design system.

## Design vs Implementation

| Element | Design | Implementation |
|---------|--------|---------------|
| Login subtitle "Sign in to your account" | 16px (`$font-base`) | 14px (0.875rem) |
| Register subtitle "Create your account" | 16px (`$font-base`) | 14px (0.875rem) |

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss` — `h2.subtitle` font-size: 0.875rem
- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss` — `.subtitle` font-size: 0.875rem

## Recommended Fix

Change subtitle font-size from `0.875rem` (14px) to `1rem` (16px) in both login and register SCSS files.
