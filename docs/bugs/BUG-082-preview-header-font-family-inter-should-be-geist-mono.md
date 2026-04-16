# BUG-082: Preview header font-family inherits Inter but should be Geist Mono

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR Editor preview header)

## Summary

`.preview-header` does not set `font-family`, so the "Preview" label text inherits `Inter` from the global styles. The design specifies `fontFamily: "Geist Mono"` for this label.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:194` — `.preview-header`

## Recommended Fix

Add `font-family: 'Geist Mono', 'Fira Code', 'JetBrains Mono', monospace;` to `.preview-header`.
