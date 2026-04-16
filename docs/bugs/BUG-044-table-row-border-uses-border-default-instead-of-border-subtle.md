# BUG-044: Table row borders use $border-default instead of $border-subtle

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 21 - Table row border audit)

## Summary

Table row separator borders (between data rows) use `#2E3142` (`$border-default`) but the design specifies `#1F2231` (`$border-subtle`) for these dividers. The design uses the subtler border intentionally to create visual hierarchy: stronger `$border-default` for card edges and major boundaries, lighter `$border-subtle` for internal row separators.

## Design vs Implementation

| Element | Design | Implementation |
|---------|--------|---------------|
| Table row `border-bottom` | `#1F2231` (`$border-subtle`) | `#2E3142` (`$border-default`) |

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:142` — `.adr-table td` border-bottom

## Recommended Fix

Change `.adr-table td` border-bottom color from `#2E3142` to `#1F2231`.
