# BUG-008: ADR author is always "Unknown" due to JWT claim type mapping

**Severity:** High  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 8)

## Summary

When creating an ADR, the `authorId` is always set to an empty string, causing the author to display as "Unknown" in the ADR list. The root cause is that `JwtSecurityTokenHandler.ValidateToken()` maps the standard JWT `sub` claim to `ClaimTypes.NameIdentifier` (a long Microsoft URI), but `VerifyAccessToken()` looks for it using `JwtRegisteredClaimNames.Sub` ("sub"). Since the claim is stored under a different key, `FindFirstValue("sub")` returns null.

## Root Cause

In `TokenService.cs:89`:
```csharp
UserId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty
```

`JwtRegisteredClaimNames.Sub` = `"sub"`, but after `ValidateToken()`, the claim is stored as `ClaimTypes.NameIdentifier` = `"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"`.

Same issue affects `email` (mapped to `ClaimTypes.Email`).

## Affected File

- `src/ArchQ.Infrastructure/Identity/TokenService.cs:80-94` — `VerifyAccessToken` method

## Recommended Fix

Disable inbound claim type mapping by setting `handler.MapInboundClaims = false` before calling `ValidateToken()`. This preserves the original JWT claim names.
