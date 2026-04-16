# BUG-013: Protected routes accessible without authentication

**Severity:** High  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 13)

## Summary

After logging out, navigating directly to `/adrs`, `/adrs/new`, or `/adrs/:id/edit` still loads the page instead of redirecting to `/login`. There are no Angular route guards (`canActivate`) on any protected routes. Unauthenticated users can view the ADR list page (though data calls fail silently due to missing auth cookies).

## Steps to Reproduce

1. Login to the application
2. Click Logout (redirects to `/login` correctly)
3. Navigate to `http://localhost:4200/adrs`
4. The ADR list page loads instead of redirecting to login

## Affected Files

- `src/ArchQ.Web/src/app/app.routes.ts` — no `canActivate` guards
- No auth guard file exists in the project

## Recommended Fix

Create an `AuthGuard` that checks `AuthService.isLoggedIn()` and add it to all protected routes (adrs, adrs/new, adrs/:id/edit, tenants/*).
