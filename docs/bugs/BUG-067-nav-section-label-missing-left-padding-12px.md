# BUG-067: Nav section label missing left padding 12px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - Sidebar navigation label)

## Summary

`.nav-section-label` has no left padding, so the "NAVIGATION" text starts flush left within the sidebar. The design specifies the label wrapper with `padding-left: 12px` to align it with the left edge of nav item icons (which also have 12px left padding).

## Affected File

- `src/ArchQ.Web/src/app/app.component.scss:45` — `.nav-section-label`

## Recommended Fix

Add `padding-left: 0.75rem` to `.nav-section-label`.
