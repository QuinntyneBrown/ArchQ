# BUG-017: Sidebar missing ArchQ temple logo icon

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 17)

## Summary

The sidebar header shows only "ArchQ" text. The design spec (Ae0g2.png) shows a blue temple/building SVG icon before the "ArchQ" text, matching the login page branding. The login page already has this icon, but it was not added to the sidebar.

## Affected File

- `src/ArchQ.Web/src/app/app.component.html:8-10` — sidebar-header has only `<span class="logo-text">ArchQ</span>`

## Recommended Fix

Add the same temple SVG icon used on the login page to the sidebar-header, styled in blue (#3b82f6).
