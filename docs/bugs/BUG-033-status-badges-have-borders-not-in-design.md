# BUG-033: Status badges have borders that are not in the design

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 10 - Status badge audit)

## Summary

Status badges (Draft, In Review, Approved, Rejected, Superseded) have a `border: 1px solid` with 30% opacity of the status color. The design system's Badge components do NOT have borders - they use only a tinted background fill. This makes badges appear more outlined/stroked than the design intends.

Additionally, badge padding is `2px 8px` instead of the design's `4px 12px`.

## Design vs Implementation

| Property | Design | Implementation |
|----------|--------|---------------|
| Border | None | `1px solid rgba(color, 0.3)` |
| Padding | `4px 12px` | `2px 8px` (0.125rem 0.5rem) |

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:165-197`
- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss` (status-badge)

## Recommended Fix

Remove `border` declarations from all `.status-*` badge rules and change padding from `0.125rem 0.5rem` to `0.25rem 0.75rem`.
