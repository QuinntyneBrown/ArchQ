# BUG-070: Mobile editor body padding 20px should be 16px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - Mobile ADR Editor)

## Summary

`.markdown-editor` uses `1.25rem` (20px) padding on all viewports. The desktop design specifies 20px which is correct, but the mobile design specifies 16px (`1rem`). A mobile override is needed.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:296` — `@media (max-width: 768px)` section

## Recommended Fix

Add `.markdown-editor { padding: 1rem; }` inside the existing `@media (max-width: 768px)` block.
