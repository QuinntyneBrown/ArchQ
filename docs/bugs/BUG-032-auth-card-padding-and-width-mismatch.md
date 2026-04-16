# BUG-032: Login and Register card padding and width don't match design

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 8 — spacing audit)

## Summary

Both the Login and Register card components use `padding: 2rem 2.5rem` (32px 40px) and `max-width: 27.5rem` (440px). The design spec specifies `padding: 40` (40px all sides) and `width: 420` (420px). The top and bottom padding are 8px too small and the card is 20px too wide.

## Design vs Actual

| Property | Design | Actual |
|----------|--------|--------|
| Padding | 40px all sides | 32px top/bottom, 40px left/right |
| Width | 420px | 440px |

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss:9-11`
- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss:9-11`

## Recommended Fix

```scss
.login-card, .register-card {
  max-width: 26.25rem; /* 420px */
  padding: 2.5rem;     /* 40px all sides */
}
```
