# BUG-001: Angular frontend API port mismatch causes total backend communication failure

**Severity:** Critical  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing  

## Summary

All Angular service files hardcode the API base URL to `http://localhost:5000`, but the local development API server (via `dotnet run`) listens on `http://localhost:5299` as configured in `launchSettings.json`. This causes every API request to fail with `net::ERR_CONNECTION_REFUSED`, making the entire application non-functional in local development.

## Affected Files

| File | Hardcoded URL |
|------|--------------|
| `src/ArchQ.Web/src/app/core/services/auth.service.ts:44` | `http://localhost:5000/api/auth` |
| `src/ArchQ.Web/src/app/core/services/adr.service.ts:8` | `http://localhost:5000/api/tenants` |
| `src/ArchQ.Web/src/app/core/services/org.service.ts:22` | `http://localhost:5000/api/orgs` |
| `src/ArchQ.Web/src/app/core/services/tenant.service.ts:8` | `http://localhost:5000/api/tenants` |

## Steps to Reproduce

1. Start Couchbase via `docker compose up -d couchbase`
2. Start the API via `dotnet run --project src/ArchQ.Api` (listens on port 5299)
3. Start Angular via `cd src/ArchQ.Web && npx ng serve` (serves on port 4200)
4. Open `http://localhost:4200/login`
5. Enter any email and password and click "Sign In"

## Expected Result

The login request is sent to the running API on port 5299 and receives a proper response (success or validation error).

## Actual Result

The request is sent to `http://localhost:5000/api/auth/login`, which is not listening, resulting in:
- Network error: `net::ERR_CONNECTION_REFUSED`
- User sees: "An unexpected error occurred"

## Root Cause

Port 5000 is the Docker Compose mapped port (`docker-compose.yml` line 23: `"5000:8080"`), not the local development port. The Angular services should reference port 5299, or better yet, use an Angular environment configuration / proxy so the correct port is used in each context.

## Recommended Fix

Introduce an Angular `environment.ts` file with a configurable `apiUrl` and update all services to use it. For local dev, set `apiUrl` to `http://localhost:5299/api`. Alternatively, configure an Angular dev server proxy (`proxy.conf.json`) to forward `/api` requests to `http://localhost:5299`.
