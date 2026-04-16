# BUG-028: Editor header title wraps to multiple lines on mobile

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 4 — mobile editor audit)

## Summary

On mobile viewports (375px), the ADR editor top bar title displays the full text "Editing ADR-001: Use Event-Driven Architecture for Order Processing" which wraps across 3+ lines, consuming excessive vertical space. The design spec (`docs/ui-design.pen`, node `qLVDf`) shows a compact single-line header: "← Edit ADR-003 [Save]".

## Steps to Reproduce

1. Log in and navigate to edit an ADR with a long title
2. Resize viewport to 375×812 (iPhone)
3. Observe the top bar header area

## Expected Result

The header should be a single line with the ADR number only (e.g., "Edit ADR-003") and a Save button, as shown in the mobile design spec.

## Actual Result

The header shows "Editing ADR-001: Use Event-Driven Architecture for Order Processing" wrapping across 3 lines, plus the Draft badge, Save Draft button, and Cancel link on a second row. The header area consumes ~120px of vertical space instead of ~44px.

## Root Cause

The `.top-bar-title` element has no text truncation. In the mobile media query (`max-width: 768px`), the `.top-bar` switches to `flex-direction: column` but the title text is unconstrained, allowing it to wrap freely.

## Affected Files

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:267-282`

## Recommended Fix

Add text truncation to `.top-bar-title` on mobile:

```scss
@media (max-width: 768px) {
  .top-bar-title {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    max-width: 60vw;
  }
}
```
