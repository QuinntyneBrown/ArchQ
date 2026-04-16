# BUG-042: Sidebar logo gap between icon and text is 8px instead of 10px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 18 — spacing audit)

## Summary

The sidebar header (ArchQ logo row) uses `gap: 0.5rem` (8px) between the icon and text. The design spec (`sideLogoRow`) specifies `gap: 10` (10px).

## Affected Files

- `src/ArchQ.Web/src/app/app.component.scss` — `.sidebar-header { gap: 0.5rem; }`

## Recommended Fix

Change `gap: 0.5rem` to `gap: 0.625rem` (10px).
