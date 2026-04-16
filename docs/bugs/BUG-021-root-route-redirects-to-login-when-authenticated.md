# BUG-021: Root route redirects logged-in users to /login instead of /adrs

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 22)

## Summary

When an authenticated user navigates to the root URL (`/`), they are redirected to `/login` instead of `/adrs`. The route config has a static `redirectTo: '/login'` that doesn't consider authentication state.

## Steps to Reproduce

1. Login to the app (redirects to `/adrs`)
2. Navigate to `http://localhost:4200/`
3. User is sent to `/login` despite being logged in

## Affected File

- `src/ArchQ.Web/src/app/app.routes.ts:19` — `{ path: '', redirectTo: '/login', pathMatch: 'full' }`

## Recommended Fix

Change the default redirect to `/adrs`. The auth guard will redirect unauthenticated users from `/adrs` to `/login`, so the flow becomes:
- Logged in: `/` → `/adrs` (guard allows) 
- Logged out: `/` → `/adrs` → `/login` (guard redirects)
