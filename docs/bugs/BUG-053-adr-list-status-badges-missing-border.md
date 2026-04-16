# BUG-053: ADR list status badges missing border

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 28)

## Summary

The ADR list page status badges lost their `border` declarations. The design system Badge components specify a 1px solid border with 30% opacity of the badge color. Without borders, badges appear flat.

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.status-draft` through `.status-superseded`
