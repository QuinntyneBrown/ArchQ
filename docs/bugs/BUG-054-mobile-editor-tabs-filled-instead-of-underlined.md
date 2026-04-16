# BUG-054: Mobile editor tabs use filled button style instead of underlined tabs

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 32 - Mobile editor tab audit)

## Summary

The mobile editor's Edit/Preview tabs render as filled button-style toggles (active tab gets blue background with white text). The design shows them as text tabs where the active tab has blue text with a 2px blue bottom border underline, and the inactive tab has disabled-color text with no background.

## Design vs Implementation

| Property | Design (Active) | Implementation (Active) |
|----------|----------------|------------------------|
| Background | transparent | `#0062FF` (filled) |
| Text color | `#0062FF` | `#fff` (white) |
| Bottom border | `2px solid #0062FF` | none (border-color matches bg) |
| Font-weight | 600 | inherited |

| Property | Design (Inactive) | Implementation (Inactive) |
|----------|------------------|--------------------------|
| Background | transparent | transparent |
| Text color | `#5C5F6E` (`$text-disabled`) | `#9ca3af` (`$text-secondary`) |
| Border | none | `1px solid #2E3142` |

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:128-142` — `.tab-btn` and `.tab-btn.active`

## Recommended Fix

Restyle `.tab-btn` as transparent text tabs with underline, not filled buttons.
