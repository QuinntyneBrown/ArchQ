# BUG-040: Org switcher text font-weight is 400 instead of 500

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 16 — typography audit)

## Summary

The org switcher name text uses `font-weight: 400` (normal) but the design specifies `fontWeight: 500` (medium). This makes the org name appear lighter than intended.

## Design vs Actual

| Property | Design | Actual |
|----------|--------|--------|
| Font-weight | 500 | 400 |
| Color | #F0F1F5 | #FFFFFF |

## Affected Files

- `src/ArchQ.Web/src/app/shared/components/org-switcher/org-switcher.component.scss` — `.org-pill`

## Recommended Fix

Add `font-weight: 500` to the `.org-pill` or `.org-name` class.
