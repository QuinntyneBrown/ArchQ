# BUG-048: Editor preview pane background is lighter than edit pane (inverted contrast)

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 23 — editor panel audit)

## Summary

The markdown editor preview pane uses `#242736` (elevated/lighter) as its background. The design specifies `#0F1117` (dark primary). This inverts the intended contrast: the design puts the edit pane on a surface-color background and the preview on the darkest background to visually separate them. The actual app makes the preview lighter, reducing the visual distinction.

## Design vs Actual

| Element | Design | Actual |
|---------|--------|--------|
| Edit pane | `#1A1D27` (surface) | transparent |
| Preview pane | `#0F1117` (dark primary) | `#242736` (elevated) |
| Preview header | `#242736` (elevated) | `#242736` ✓ |

## Affected Files

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss` — `.preview-pane`

## Recommended Fix

Change `.preview-pane { background-color: #242736; }` to `#0F1117`.
