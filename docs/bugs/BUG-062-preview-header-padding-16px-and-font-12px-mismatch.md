# BUG-062: Preview header horizontal padding 16px and font-size 12px mismatch

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR Editor preview pane)

## Summary

`.preview-header` in the ADR editor has two discrepancies from the design:
1. Horizontal padding is `1rem` (16px) but design specifies 12px (`0.75rem`)
2. Font-size is `0.75rem` (12px) but design specifies 11px (`0.6875rem`)

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:192` — `.preview-header`

## Recommended Fix

Change `.preview-header`:
- padding from `0.5rem 1rem` to `0.5rem 0.75rem`
- font-size from `0.75rem` to `0.6875rem`
