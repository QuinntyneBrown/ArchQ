# BUG-004: Sidebar does not collapse on mobile/tablet viewports

**Severity:** High  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 4)

## Summary

The application sidebar is always visible at a fixed 16rem (256px) width regardless of viewport size. On mobile (390px), this consumes 65% of the screen, leaving the main content area in an unusable 134px-wide column. The design spec shows the sidebar should be hidden on mobile/tablet and replaced by a hamburger menu.

## Design Spec vs Actual

**Mobile design spec** (`docs/designs/exports/8U5mr.png`):
- No sidebar visible
- Hamburger menu icon in top-left of header bar
- Full-width content area with card-based ADR layout

**Tablet design spec** (`docs/designs/exports/lAy71.png`):
- No sidebar visible
- Hamburger menu + "ArchQ" + "Acme Corp" chip in header bar
- Full-width content area with compact table

**Actual implementation:**
- Sidebar always visible at 256px width on ALL viewports
- No hamburger menu
- Content area squeezed to remaining space
- Text wraps badly on mobile, layout is broken

## Affected Files

- `src/ArchQ.Web/src/app/app.component.scss` — sidebar has no `@media` queries
- `src/ArchQ.Web/src/app/app.component.html` — no hamburger menu toggle element

## Steps to Reproduce

1. Login to the application
2. Resize browser to 390px width (or use mobile device emulation)
3. Observe the sidebar taking 65% of the viewport

## Recommended Fix

Add `@media` breakpoints to hide the sidebar at mobile/tablet widths (<= 768px), add a hamburger menu button that toggles sidebar visibility as an overlay, and make the main content area full-width on smaller viewports.
