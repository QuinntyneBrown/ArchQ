# BUG-025: ADR list table columns truncated at tablet viewport width

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 1)

## Summary

At the standard tablet viewport width (768px), the ADR list table's "Modified" column header is visually truncated to "MODIFIE" and date values like "Apr 16, 2026" are clipped to "Apr 16, 2". The table columns exceed the available container width, causing the rightmost column content to be cut off.

## Steps to Reproduce

1. Log in and navigate to the ADR list page (`/adrs`)
2. Ensure at least one ADR exists
3. Resize the browser viewport to 768×1024 (standard iPad portrait)
4. Observe the ADR table

## Expected Result

Per the tablet design spec (`docs/ui-design.pen`, node `lAy71`), all five columns (Number, Title, Status, Author, Modified) should fit within the 768px viewport with properly proportioned widths and no clipping.

## Actual Result

- The "MODIFIED" column header is truncated to "MODIFIE" (missing the final "D")
- Date values are clipped (e.g., "Apr 16, 2026" → "Apr 16, 2")
- The ADR number "ADR-001" wraps to two lines
- The Author name "Admin User" wraps to two lines

## Root Cause

The SCSS media query for condensed table layout targets `(min-width: 576px) and (max-width: 767px)` at line 420 of `adr-list.component.scss`, which **excludes 768px** (the standard tablet width). At 768px:

1. The "tablet range" media query (576–767px) does NOT apply, so no column is hidden
2. The mobile breakpoint (≤575px) does NOT apply, so the table is shown instead of cards
3. All 5 columns render with desktop-proportioned widths that exceed the available space
4. The `.title-cell` has `max-width: 20rem` (320px) — nearly half the viewport — leaving insufficient room for other columns

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:399-425`

## Recommended Fix

Add responsive column sizing for the full tablet range (768px–1024px). Options:

1. **Extend the existing media query** from `max-width: 767px` to `max-width: 1024px` to hide the Modified column across the full tablet range, OR
2. **Add a new tablet-specific rule** that reduces `.title-cell` max-width and adds `min-width` constraints to the Modified column:

```scss
@media (min-width: 768px) and (max-width: 1024px) {
  .title-cell {
    max-width: 14rem;
  }
  .adr-table th,
  .adr-table td {
    padding: 0.75rem 0.5rem;
  }
}
```

## Screenshot Evidence

- Design spec: `tests/screenshots/` — tablet design shows all columns fitting at 768px
- Actual: `tests/screenshots/adr-list-tablet-actual.png` — columns truncated
