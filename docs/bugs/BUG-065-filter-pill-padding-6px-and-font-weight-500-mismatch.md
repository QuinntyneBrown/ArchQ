# BUG-065: Filter pill vertical padding 6px and font-weight 500 mismatch

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - Mobile ADR List filter pills)

## Summary

`.pill` in the mobile ADR list has two discrepancies from the design:
1. Vertical padding is `0.375rem` (6px) but design specifies 4px (`0.25rem`)
2. Font-weight is `500` but design specifies `normal` (400) for inactive pills and `600` for the active pill

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:103` — `.pill`
- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:116` — `.pill-active`

## Recommended Fix

1. Change `.pill` padding from `0.375rem 0.75rem` to `0.25rem 0.75rem`
2. Change `.pill` font-weight from `500` to `normal`
3. Add `font-weight: 600` to `.pill-active`
