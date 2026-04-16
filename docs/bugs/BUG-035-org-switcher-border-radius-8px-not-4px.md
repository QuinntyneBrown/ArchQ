# BUG-035: Org switcher button has 8px border-radius instead of 4px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 11 — border-radius audit)

## Summary

The org switcher button (`.org-pill`) uses `border-radius: 0.5rem` (8px) while the design spec specifies `cornerRadius: 4` (4px). All other interactive elements (inputs, buttons, nav items, filters) correctly use 4px. The org switcher is the only element with an inconsistent corner radius.

## Affected Files

- `src/ArchQ.Web/src/app/shared/components/org-switcher/org-switcher.component.scss:13`

## Recommended Fix

Change `border-radius: 0.5rem` to `border-radius: 0.25rem` (4px).
