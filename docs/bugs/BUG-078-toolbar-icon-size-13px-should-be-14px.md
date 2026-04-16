# BUG-078: Toolbar icon size 13px should be 14px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR Editor toolbar icons)

## Summary

`.toolbar-btn` uses `font-size: 0.8125rem` (13px) for toolbar icons but the design specifies 14x14 icons (`0.875rem`). The icons are 1px smaller than the design spec.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:230` — `.toolbar-btn`

## Recommended Fix

Change `.toolbar-btn` font-size from `0.8125rem` to `0.875rem`.
