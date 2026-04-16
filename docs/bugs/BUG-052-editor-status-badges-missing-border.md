# BUG-052: Editor status badges missing border (inconsistent with list badges)

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 27 — editor badge audit)

## Summary

The editor page status badges (Draft, Approved, etc.) have no border, while the ADR list page badges correctly include `border: 1px solid ...` matching the design. The editor badge `border` declarations were removed, making them look flat compared to the list view.

## Affected Files

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss` — `.status-draft` through `.status-superseded`

## Recommended Fix

Add `border: 1px solid rgba(...)` to each status class in the editor SCSS, matching the list component.
