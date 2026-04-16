# BUG-072: Tablet table header font-size 12px should be 11px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - Tablet ADR List table header)

## Summary

The tablet design specifies table header text at 11px (`0.6875rem`), but the SCSS only has the desktop value of 12px (`0.75rem`) with no tablet override. The header text should be slightly smaller on tablet.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — tablet media query (768-1024px)

## Recommended Fix

Add `font-size: 0.6875rem` to `.adr-table th` in the tablet media query.
