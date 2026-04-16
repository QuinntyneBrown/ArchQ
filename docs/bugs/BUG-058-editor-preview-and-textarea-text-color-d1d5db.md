# BUG-058: Editor preview and textarea text color is #D1D5DB instead of $text-primary

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 37 - Editor body text audit)

## Summary

The editor's markdown preview panel and textarea use `#d1d5db` for body text color. This is a non-token color. Content body text should use `$text-primary` (`#F0F1F5`) for consistency with the design system.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss` — `.preview-content` and `.markdown-editor`

## Recommended Fix

Change `color: #d1d5db` to `color: #F0F1F5` in both rules.
