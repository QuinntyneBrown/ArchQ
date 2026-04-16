# BUG-080: Preview header text-transform uppercase not in design

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR Editor preview header)

## Summary

`.preview-header` applies `text-transform: uppercase` and `letter-spacing: 0.05em`, rendering "Preview" as "PREVIEW". The design shows the label as "Preview" in sentence case with no text-transform. The design uses explicit uppercase in content strings (e.g., "NAVIGATION") when uppercase is intended; "Preview" is in sentence case.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:195` — `.preview-header`

## Recommended Fix

Remove `text-transform: uppercase` and `letter-spacing: 0.05em` from `.preview-header`.
