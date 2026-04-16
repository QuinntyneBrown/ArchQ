# BUG-055: Editor text colors use #FFFFFF instead of $text-primary #F0F1F5

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 33 - Editor text color audit)

## Summary

Several text elements in the ADR editor use `#ffffff` (pure white) instead of `#F0F1F5` (`$text-primary`). This is the same pattern fixed in BUG-040 for auth pages and ADR list but not yet applied to the editor component.

## Affected Properties

- `.title-input color: #ffffff` — ADR title input text
- `.markdown-body h1, h2, h3 color: #ffffff` — rendered markdown headings

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss`

## Recommended Fix

Change `color: #ffffff` to `color: #F0F1F5` for title input and markdown headings.
