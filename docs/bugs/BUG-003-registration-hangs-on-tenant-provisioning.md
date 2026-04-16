# BUG-003: Registration endpoint hangs during Couchbase tenant provisioning

**Severity:** High  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 3)

## Summary

The `/api/auth/register` endpoint hangs indefinitely when creating a new tenant (organization). The handler creates the Couchbase scope, collections, and indexes successfully, but the HTTP response is never returned to the client. Requests with an existing email (which skip provisioning) return instantly, confirming the issue is in the provisioning path.

## Root Cause

The `CouchbaseProvisioner.CreateIndexesAsync()` method creates 16 N1QL indexes (10 primary + 6 secondary) sequentially on newly created collections. While the indexes are created successfully in Couchbase, the Couchbase .NET SDK's `scope.QueryAsync()` call appears to hang on the response after completing the query. This causes the entire registration request to never complete.

## Affected Files

- `src/ArchQ.Infrastructure/Persistence/CouchbaseProvisioner.cs:63-91` — `CreateIndexesAsync` method
- `src/ArchQ.Application/Tenants/Commands/CreateTenant/CreateTenantCommandHandler.cs:32-34` — calls provisioner

## Steps to Reproduce

1. Start the system locally (Couchbase + API + Angular)
2. Send a POST to `/api/auth/register` with a new email and organization
3. The request body is accepted, all database operations complete, but the response never returns

```bash
curl -v --max-time 30 -X POST http://localhost:5299/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"fullName":"Test","email":"test@example.com","password":"S3cur3P@ss!","organizationName":"NewCorp"}'
# Times out after 30 seconds with no response
```

## Evidence

- Scope, collections (10), and indexes (16) are all created in Couchbase
- Tenant document stored in `system.tenants`
- User document stored in the new scope
- Global user mapping created
- Verification email logged
- But HTTP response never sent to the client

## Workaround

Registrations with existing emails return instantly (early return on line 55-58 of `RegisterCommandHandler`).

## Recommended Fix

Add a `CancellationToken` and timeout to the `QueryOptions` in `CreateIndexesAsync`. Consider also creating indexes asynchronously (fire-and-forget or background job) rather than blocking the registration response.
