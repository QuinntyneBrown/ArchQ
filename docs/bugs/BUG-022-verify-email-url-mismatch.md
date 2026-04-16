# BUG-022: Verification email URL doesn't match Angular route

**Severity:** High  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 23)

## Summary

The verification email sent after registration contains the URL `http://localhost:4200/auth/verify-email?token=...`, but the Angular route is defined as `{ path: 'verify-email' }` which resolves to `/verify-email` (no `/auth/` prefix). When users click the verification link, they land on a non-existent route and cannot verify their email.

## Affected Files

- `src/ArchQ.Infrastructure/Email/SmtpEmailService.cs` — generates URL with `/auth/verify-email`
- `src/ArchQ.Web/src/app/app.routes.ts:14` — route is `path: 'verify-email'` (no `auth/` prefix)

## Recommended Fix

Change the email service URL from `/auth/verify-email` to `/verify-email` to match the Angular route.
