# BUG-030: Sidebar uses wrong background and border colors

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 6 — color audit)

## Summary

The sidebar background color is `#12141f` instead of the design system's surface color `#1A1D27`, and the border color is `#1e2030` instead of `#2E3142`. This makes the sidebar noticeably darker than the content cards, which use the correct `#1A1D27`. In the design, the sidebar and cards share the same surface color for a cohesive appearance.

## Design Spec vs Actual

| Property | Design | Actual |
|----------|--------|--------|
| Background | `#1A1D27` | `#12141f` |
| Border-right | `1px solid #2E3142` | `1px solid #1e2030` |
| Width | 260px | 256px |
| Padding | 24px 16px | 16px |

## Affected Files

- `src/ArchQ.Web/src/app/app.component.scss:6-14` (`.app-sidebar`)

## Recommended Fix

```scss
.app-sidebar {
  width: 16.25rem;  /* 260px */
  background-color: #1A1D27;
  border-right: 1px solid #2E3142;
  padding: 1.5rem 1rem;
}
```
