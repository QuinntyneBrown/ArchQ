# BUG-045: Mobile ADR card title font-size is 15px instead of 14px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 20 — mobile card audit)

## Summary

The mobile ADR card title uses `font-size: 0.9375rem` (15px) but the design specifies 14px. This 1px difference compounds across all cards in the list.

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.card-title`

## Recommended Fix

Change `font-size: 0.9375rem` to `font-size: 0.875rem` (14px).
