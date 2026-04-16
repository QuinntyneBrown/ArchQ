# BUG-007: Session state lost on page refresh

**Severity:** High  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 7)

## Summary

When a logged-in user refreshes the page (or navigates via hard URL), all session state is lost. The Angular signals storing `currentUser`, `currentTenant`, and `memberships` are cleared, and the app shows "No tenant selected". The user appears logged out despite having valid HttpOnly auth cookies.

This happens because:
1. The `AuthService` stores session state only in-memory Angular signals
2. There is no `APP_INITIALIZER` or auth guard that restores session state on app boot
3. The `/api/auth/refresh` endpoint only returns `{ message: "Token refreshed." }` without user/tenant data

## Impact

- ADR list shows empty after page refresh (no tenant context for API calls)
- Subtitle shows "Manage and track architectural decisions for" (missing org name)
- "No tenant selected" error appears

## Affected Files

- `src/ArchQ.Api/Controllers/AuthController.cs:64-79` — refresh endpoint returns no session data
- `src/ArchQ.Web/src/app/core/services/auth.service.ts` — no session restore on init
- `src/ArchQ.Web/src/app/app.config.ts` — no APP_INITIALIZER

## Recommended Fix

1. Update the refresh endpoint to return user/tenant/memberships data (same shape as login response)
2. Add an `APP_INITIALIZER` in Angular that calls `/api/auth/refresh` on startup and populates the auth signals
