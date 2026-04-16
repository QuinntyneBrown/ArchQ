# BUG-061: Toolbar button size 32px should be 28px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR Editor toolbar)

## Summary

`.toolbar-btn` in the ADR editor uses `2rem` (32px) for width and height. The design specifies toolbar button containers at 28px (`1.75rem`). This makes the toolbar buttons 4px larger than intended, affecting visual density.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:219` — `.toolbar-btn`

## Recommended Fix

Change `.toolbar-btn` width and height from `2rem` to `1.75rem`.
