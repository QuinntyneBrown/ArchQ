# BUG-075: Editor top bar left gap 12px should be 8px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR Editor top bar)

## Summary

`.top-bar-left` in the ADR editor uses `gap: 0.75rem` (12px) but the design specifies gap 8 (`0.5rem`) between the back arrow, title, and status badge.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:30` — `.top-bar-left`

## Recommended Fix

Change `.top-bar-left` gap from `0.75rem` to `0.5rem`.
