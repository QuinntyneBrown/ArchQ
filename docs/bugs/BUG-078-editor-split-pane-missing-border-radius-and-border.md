# BUG-078: Editor split pane missing border-radius, border, and clip

## Summary
The ADR editor's split pane (containing the markdown editor and preview) is missing the border-radius, border, and overflow clipping specified in the design spec.

## Actual Behavior
- `border-radius`: 0px
- `border`: none
- `overflow`: visible

## Expected Behavior (from design spec)
- `border-radius`: $radius-md (8px)
- `border`: 1px solid $border-default (#2E3142)
- `overflow`: hidden (clip: true in design)

## Root Cause
The `.split-pane` CSS class only has flex layout properties but no border-radius, border, or overflow styles.

## Fix
Add `border-radius: 0.5rem`, `border: 1px solid #2E3142`, and `overflow: hidden` to `.split-pane`.

## Severity
Low — visual polish, the split pane should have rounded corners with a subtle border

## Steps to Reproduce
1. Log in and navigate to the ADR editor (/adrs/new or /adrs/:id/edit)
2. Observe the split pane (editor + preview) has no rounded corners or border
3. Design spec shows the Split Pane Editor frame with cornerRadius $radius-md, stroke border-default
