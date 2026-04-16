# BUG-081: Preview header font-weight 600 should be normal

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR Editor preview header)

## Summary

`.preview-header` uses `font-weight: 600` but the design specifies `fontWeight: "normal"` for the preview label text. The label should use normal (400) weight, not semi-bold.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:194` — `.preview-header`

## Recommended Fix

Change `.preview-header` font-weight from `600` to `normal`.
