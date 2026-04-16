# BUG-046: "New ADR" button padding 8px 16px instead of 10px 20px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 21 — button audit)

## Summary

The "+ New ADR" button on the ADR list page uses `padding: 0.5rem 1rem` (8px 16px) but the design system's Button/Primary component specifies `padding: [10, 20]` (10px 20px). The auth page buttons correctly use 10px 20px, creating an inconsistency.

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.btn-primary`

## Recommended Fix

Change `padding: 0.5rem 1rem` to `padding: 0.625rem 1.25rem` (10px 20px).
