# BUG-053: Editor top bar title is 16px instead of design's 18px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 31 - Editor top bar audit)

## Summary

The ADR editor top bar title (e.g. "Editing ADR-003: CQRS Pattern") uses `font-size: 1rem` (16px) but the design specifies `fontSize: 18` (`$font-lg`). The title color is also `#ffffff` instead of `#F0F1F5` (`$text-primary`).

## Design vs Implementation

| Property | Design | Implementation |
|----------|--------|---------------|
| Font-size | 18px (`$font-lg`) | 16px (1rem) |
| Color | `#F0F1F5` (`$text-primary`) | `#ffffff` |

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss` — `.top-bar-title`

## Recommended Fix

Change `.top-bar-title` font-size to `1.125rem` (18px) and color to `#F0F1F5`.
