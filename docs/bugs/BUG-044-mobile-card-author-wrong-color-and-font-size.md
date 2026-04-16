# BUG-044: Mobile ADR card author text wrong color and font-size

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 20 — color audit)

## Summary

The mobile ADR card author name uses `#9CA3AF` (light gray) at 13px, but the design specifies `#5C5F6E` (darker gray) at 12px. The lighter color makes the author text more prominent than intended, reducing the visual hierarchy between the title and metadata.

## Design vs Actual

| Property | Design | Actual |
|----------|--------|--------|
| Color | `#5C5F6E` | `#9CA3AF` |
| Font-size | 12px | 13px |

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.card-author`

## Recommended Fix

```scss
.card-author {
  color: #5C5F6E;
  font-size: 0.75rem; /* 12px */
}
```
