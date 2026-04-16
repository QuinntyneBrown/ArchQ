# BUG-019: Editor shows "Save Changes" instead of "Save Draft" and badge is lowercase

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 20)

## Summary

Two issues on the ADR editor page:

1. **Save button label**: When editing an existing draft ADR, the button shows "Save Changes". The design spec (Huy4L.png) shows "Save Draft" since the ADR is still in draft status.

2. **Status badge capitalization**: The editor's `.status-badge` shows lowercase "draft" because the `text-transform: capitalize` fix from BUG-010 was only applied to the list page, not the editor.

## Affected Files

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.html:27` — button text logic
- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:45` — `.status-badge` missing capitalize
