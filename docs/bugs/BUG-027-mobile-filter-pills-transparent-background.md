# BUG-027: Mobile filter pills have transparent background instead of $bg-elevated

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 4 - Mobile ADR List audit)

## Summary

The inactive mobile filter pills on the ADR List page have `background: transparent` instead of the design system's `$bg-elevated` (`#242736`). The pills also use the wrong border color `#3A3F54` instead of `$border-default` (`#2E3142`). This makes the pills appear hollow/ghosted rather than as solid elevated surface elements per the design.

## Design vs Implementation

| Property | Design | Implementation |
|----------|--------|---------------|
| Inactive pill background | `#242736` (`$bg-elevated`) | `transparent` |
| Inactive pill border | `#2E3142` (`$border-default`) | `#3A3F54` |
| Inactive pill font-weight | `normal` (400) | `500` |

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:101-110` — `.pill` class

## Recommended Fix

Change `.pill` background from `transparent` to `#242736` and border from `#3a3f54` to `#2E3142`.
