# BUG-071: Tablet table horizontal padding 8px should be 16px

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - Tablet ADR List table)

## Summary

The tablet media query overrides `.adr-table th, .adr-table td` with `padding: 0.75rem 0.5rem` (12px 8px). The design specifies:
- Table header (th): `padding: 0.625rem 1rem` (10px 16px)
- Table rows (td): `padding: 0.75rem 1rem` (12px 16px)

The horizontal padding is halved (8px vs 16px design), and the header vertical padding should be 10px not 12px.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:429` — tablet media query

## Recommended Fix

Change the tablet media query to:
```scss
.adr-table th { padding: 0.625rem 1rem; }
.adr-table td { padding: 0.75rem 1rem; }
```
