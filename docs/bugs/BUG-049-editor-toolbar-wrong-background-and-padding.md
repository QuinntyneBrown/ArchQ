# BUG-049: Editor toolbar uses surface bg instead of elevated, padding too small

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 24 — editor audit)

## Summary

The markdown editor toolbar uses `#1A1D27` (surface color) as its background instead of `#242736` (elevated). This makes the toolbar blend into the edit pane instead of being visually distinct. Additionally, toolbar vertical padding is 6px instead of 8px, textarea padding is 16px instead of 20px, and preview content padding is 16px instead of 24px.

## Design vs Actual

| Element | Property | Design | Actual |
|---------|----------|--------|--------|
| Toolbar | background | `#242736` | `#1A1D27` |
| Toolbar | padding | 8px 12px | 6px 12px |
| Edit content | padding | 20px | 16px |
| Preview content | padding | 24px | 16px |

## Affected Files

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss` — `.toolbar`, `.markdown-editor`, `.preview-content`
