# BUG-005: Missing subtitle on ADR list page

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 5)

## Summary

The ADR list page heading shows only "Architecture Decision Records" but the design spec shows a subtitle below it: "Manage and track architectural decisions for [Organization Name]". This subtitle provides context about which organization's ADRs are being viewed, which is important in a multi-tenant application.

## Design Spec vs Actual

**Design spec** (`docs/designs/exports/Ae0g2.png`):
```
Architecture Decision Records
Manage and track architectural decisions for Acme Corp
```

**Actual:**
```
Architecture Decision Records
(no subtitle)
```

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.html:5`

## Recommended Fix

Add a `<p>` subtitle element after the `<h1>` that reads "Manage and track architectural decisions for {{ tenantName }}" using the tenant display name from `AuthService.currentTenant().displayName`.
