# BUG-041: Markdown editor textarea missing Geist Mono as primary font

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 17 — typography audit)

## Summary

The markdown editor textarea uses `font-family: 'Fira Code', 'Cascadia Code', ...` but the design specifies `Geist Mono` as the editor font. Since Geist Mono is already imported globally (added in BUG-033 fix), it should be the primary font in the editor's font stack.

## Affected Files

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:217`

## Recommended Fix

Add `'Geist Mono'` as the first font in the stack:
```scss
font-family: 'Geist Mono', 'Fira Code', 'Cascadia Code', 'JetBrains Mono', 'Consolas', monospace;
```
