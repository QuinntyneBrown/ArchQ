# BUG-036: Search highlight and FAB shadow use old blue in rgba form

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 13 - Remaining color audit)

## Summary

The search result text highlight and the mobile FAB button shadow use `rgba(37, 99, 235, ...)` which is the rgba form of the old Tailwind blue `#2563EB`. These were missed in BUG-024's hex-only find-and-replace. They should use the accent-primary color `#0062FF` = `rgba(0, 98, 255, ...)`.

## Affected Instances

| Element | Current | Should Be |
|---------|---------|-----------|
| Search highlight mark | `rgba(37, 99, 235, 0.3)` | `rgba(0, 98, 255, 0.3)` |
| FAB button shadow | `rgba(37, 99, 235, 0.4)` | `rgba(0, 98, 255, 0.4)` |

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:349,390`

## Recommended Fix

Replace `rgba(37, 99, 235` with `rgba(0, 98, 255` in the two affected rules.
