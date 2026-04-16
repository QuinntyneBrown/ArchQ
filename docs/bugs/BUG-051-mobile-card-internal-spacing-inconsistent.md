# BUG-051: Mobile ADR card internal spacing inconsistent with design gap: 10

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 26 — mobile spacing audit)

## Summary

The mobile ADR card's internal elements use different margin-bottom values (8px for top row, 12px for title) instead of a uniform 10px gap as specified by the design's `gap: 10`.

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.card-top-row`, `.card-title`

## Recommended Fix

Change `.card-top-row { margin-bottom: 0.5rem; }` to `0.625rem` (10px) and `.card-title { margin-bottom: 0.75rem; }` to `0.625rem` (10px).
