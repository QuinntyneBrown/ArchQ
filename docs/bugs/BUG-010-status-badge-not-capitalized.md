# BUG-010: Status badges display lowercase text instead of capitalized

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 10)

## Summary

ADR status badges display the raw lowercase value from the API (e.g., "draft", "in review") instead of the properly capitalized form shown in the design spec ("Draft", "In Review"). The design spec component library (TjWsF.png) shows all status badges with capitalized text.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.status-badge` has no `text-transform`

## Recommended Fix

Add `text-transform: capitalize` to the `.status-badge` CSS class.
