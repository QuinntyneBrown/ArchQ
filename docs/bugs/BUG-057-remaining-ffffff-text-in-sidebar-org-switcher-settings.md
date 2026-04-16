# BUG-057: Remaining #FFFFFF text in sidebar, org-switcher, and settings

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 35 - Full codebase #ffffff scan)

## Summary

Several components still use `#ffffff` for primary text on dark backgrounds where `$text-primary` (`#F0F1F5`) should be used. This is the same pattern fixed in BUG-040 and BUG-055 but for remaining components.

## Affected Locations

- `app.component.scss:32` — sidebar `.logo-text`
- `org-switcher.component.scss:14` — org switcher text
- `tenant-create.component.scss:13` — page heading
- `tenant-create.component.scss:34` — input text
- `verify-email.component.scss:32` — page heading
- `tenant-detail.component.scss:46` — input text

## Recommended Fix

Change `color: #ffffff` to `color: #F0F1F5` in these non-hover, non-button text rules.
