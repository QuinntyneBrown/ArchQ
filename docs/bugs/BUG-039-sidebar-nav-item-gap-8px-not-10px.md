# BUG-039: Sidebar nav item gap between icon and text is 8px instead of 10px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 15 — spacing audit)

## Summary

The sidebar nav items (e.g., "ADR Records") have `gap: 0.5rem` (8px) between the icon and text. The design spec (`NavItem/Active` component) uses `gap: 10`. This 2px difference makes the icon and text feel slightly more cramped than designed.

## Affected Files

- `src/ArchQ.Web/src/app/app.component.scss` — `.nav-item { gap: 0.5rem; }`

## Recommended Fix

Change `gap: 0.5rem` to `gap: 0.625rem` (10px).
