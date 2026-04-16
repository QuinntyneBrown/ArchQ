# BUG-077: Mobile editor header horizontal padding 24px should be 16px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - Mobile ADR Editor header)

## Summary

The `.top-bar` in the mobile media query inherits the desktop horizontal padding of `1.5rem` (24px). The design mobile editor header specifies `padding: [12,16]` (12px 16px). A mobile override is needed to reduce horizontal padding to 16px.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:301` — `.top-bar` inside `@media (max-width: 768px)`

## Recommended Fix

Add `padding: 0.75rem 1rem` to the `.top-bar` mobile override.
