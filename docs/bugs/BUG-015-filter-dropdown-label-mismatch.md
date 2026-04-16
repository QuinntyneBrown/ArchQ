# BUG-015: Status filter dropdown shows "All" instead of "All Statuses"

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 15)

## Summary

The status filter dropdown on the ADR list shows "All" as the default option. The design spec (Ae0g2.png) shows "All Statuses" as the label, which provides clearer context about what the filter controls.

## Design Spec (Ae0g2.png)

Dropdown shows: "All Statuses ▾"

## Actual

Dropdown shows: "All ▾"

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.html:25`

## Recommended Fix

Change `<option value="All">All</option>` to `<option value="All">All Statuses</option>`.
