# BUG-011: Sidebar visible on tablet viewport (810px) instead of hidden

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 11)

## Summary

The sidebar responsive breakpoint is set to `max-width: 768px`, which hides the sidebar on mobile but not on tablet (810px). The design spec (lAy71.png) shows the sidebar should be hidden on tablet viewports too, with a hamburger menu for navigation. At 810px, the sidebar consumes 256px (31%) of the viewport, causing the "Modified" table column to be truncated.

## Design Spec

- `lAy71.png` (tablet, ~960px): No sidebar, hamburger menu in header bar, full-width table
- `Ae0g2.png` (desktop, ~1440px): Full sidebar visible

## Affected File

- `src/ArchQ.Web/src/app/app.component.scss` — `@media (max-width: 768px)` should be `@media (max-width: 1024px)`

## Recommended Fix

Increase the breakpoint from 768px to 1024px so both mobile and tablet viewports hide the sidebar.
