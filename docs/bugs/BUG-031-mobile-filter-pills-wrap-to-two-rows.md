# BUG-031: Mobile filter pills wrap to two rows instead of single scrollable row

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 7 — mobile layout audit)

## Summary

On mobile viewports (375px), the status filter pills (All, Draft, In Review, Approved, Rejected, Superseded) wrap to two rows, consuming 66px of vertical space. The design spec (`docs/ui-design.pen`, node `UCyZM`) shows pills in a single horizontal row that fits within the viewport width.

## Steps to Reproduce

1. Log in and navigate to the ADR list
2. Resize viewport to 375px width (mobile)
3. Observe the filter pill row below the search input

## Expected Result

Filter pills display in a single horizontal row, scrollable if they overflow.

## Actual Result

The 6 pills wrap to 2 rows: (All, Draft, In Review, Approved) on row 1, (Rejected, Superseded) on row 2.

## Root Cause

`.filter-pills` uses `flex-wrap: wrap` in `adr-list.component.scss:99`, allowing pills to wrap to the next line when they exceed the container width.

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:95-100`

## Recommended Fix

Change the filter pills to a single scrollable row:

```scss
.filter-pills {
  display: none;
  gap: 0.5rem;
  margin-bottom: 1rem;
  flex-wrap: nowrap;
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
}
```
