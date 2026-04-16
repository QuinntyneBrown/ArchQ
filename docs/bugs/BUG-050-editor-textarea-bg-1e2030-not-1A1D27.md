# BUG-050: Markdown editor textarea background uses non-standard #1e2030

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 25 — final color audit)

## Summary

The markdown editor textarea uses `background-color: #1e2030` which is not a design system color token. The edit pane background should be `#1A1D27` (`$bg-surface`). The `#1e2030` shade has a slightly bluer tint that doesn't match any design token.

## Affected Files

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss` — `.markdown-editor`

## Recommended Fix

Change `background-color: #1e2030` to `#1A1D27`.
