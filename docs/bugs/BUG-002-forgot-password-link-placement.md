# BUG-002: "Forgot password?" link misplaced on login page

**Severity:** Low (UI layout)  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 2)

## Summary

The "Forgot password?" link on the login page is positioned inline with the "Password" label (right-aligned in the same row), but the design spec shows it should appear below the "Sign In" button as a standalone centered link.

## Design Spec vs Actual

**Design spec** (`docs/designs/exports/RWz27.png`):
```
Email Address
[_______________]
Password
[_______________]
[    Sign In    ]
  Forgot password?     <-- centered, below button
──── OR ────
Don't have an account? Sign up
```

**Actual implementation:**
```
Email Address
[_______________]
Password       Forgot password?   <-- inline with label
[_______________]
[    Sign In    ]
──── OR ────
Don't have an account? Sign up
```

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/login/login.component.html:34-37` — `password-label-row` wraps label + link
- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss:43-58` — flex row styling

## Steps to Reproduce

1. Navigate to `http://localhost:4200/login`
2. Observe the "Forgot password?" link position

## Expected

"Forgot password?" appears centered below the "Sign In" button, per the design spec.

## Actual

"Forgot password?" appears inline right-aligned next to the "Password" label.

## Recommended Fix

Move the `<a class="forgot-password">` element from inside the `password-label-row` to after the submit button. Remove the `password-label-row` wrapper. Style the link as centered text below the button.
