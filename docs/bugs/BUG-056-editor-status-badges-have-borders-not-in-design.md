# BUG-056: Editor status badges have borders not present in design

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 34 - Editor badge audit)

## Summary

Same issue as BUG-033 but in the ADR editor component. The editor's status badge CSS rules (draft, in-review, approved, rejected, superseded) include `border: 1px solid` declarations that are not present in the design's Badge components.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:53-77`

## Recommended Fix

Remove `border` declarations from all `.status-*` rules in the editor SCSS.
