# BUG-026: Draft status badge uses wrong color (green instead of amber)

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 2 — design system audit)

## Summary

The "Draft" status badge uses green (`#22c55e`) for its text, background, and border — the same color as the "Approved" badge. The design system specifies amber (`#F59E0B`) for Draft. This makes Draft and Approved ADRs visually indistinguishable in the list, which is confusing and defeats the purpose of color-coded status badges.

Additionally, the "Approved" badge uses `#22c55e` (lime green) instead of the design-spec emerald `#10B981`, and "Superseded" uses `#6B7280` text instead of `#9CA3AF`.

## Steps to Reproduce

1. Log in and navigate to the ADR list
2. Observe the status badge on any Draft ADR
3. Compare with the design spec (`docs/ui-design.pen`, node `RMDyw` for Badge/Draft)

## Expected Result (from design system)

| Badge | Text Color | Background |
|-------|-----------|------------|
| Draft | `#F59E0B` (amber) | `rgba(245, 158, 11, 0.125)` |
| In Review | `#3B82F6` (blue) | `rgba(59, 130, 246, 0.125)` |
| Approved | `#10B981` (emerald) | `rgba(16, 185, 129, 0.125)` |
| Rejected | `#EF4444` (red) | `rgba(239, 68, 68, 0.125)` |
| Superseded | `#9CA3AF` (gray) | `rgba(107, 114, 128, 0.125)` |

## Actual Result

| Badge | Text Color | Background |
|-------|-----------|------------|
| Draft | `#22c55e` (green) | `rgba(34, 197, 94, 0.1)` |
| In Review | `#3b82f6` (blue) | `rgba(59, 130, 246, 0.1)` |
| Approved | `#22c55e` (green) | `rgba(34, 197, 94, 0.1)` |
| Rejected | `#ef4444` (red) | `rgba(239, 68, 68, 0.1)` |
| Superseded | `#6b7280` (gray) | `rgba(107, 114, 128, 0.1)` |

Draft and Approved are identical, making them indistinguishable.

## Root Cause

In `adr-list.component.scss`, `.status-draft` copies the exact same color values as `.status-approved`, both using `#22c55e`.

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:173-177` (`.status-draft`)
- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:183-187` (`.status-approved`)

## Recommended Fix

Update the badge color classes to match the design system:

```scss
.status-draft {
  color: #F59E0B;
  background-color: rgba(245, 158, 11, 0.125);
  border: 1px solid rgba(245, 158, 11, 0.3);
}
.status-approved {
  color: #10B981;
  background-color: rgba(16, 185, 129, 0.125);
  border: 1px solid rgba(16, 185, 129, 0.3);
}
.status-superseded {
  color: #9CA3AF;
  background-color: rgba(107, 114, 128, 0.125);
  border: 1px solid rgba(156, 163, 175, 0.3);
}
```
