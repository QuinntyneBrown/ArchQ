# BUG-047: Tablet toolbar missing background color and full width

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 24 - Tablet layout audit)

## Summary

The tablet/mobile top toolbar (`.app-toolbar`) that appears when the sidebar is hidden (below 1024px) is missing a background color and full-width styling. The design shows the tablet header with `$bg-surface` (`#1A1D27`) background and a bottom border `$border-default` (`#2E3142`), spanning the full viewport width. The current implementation has no background (transparent) and no explicit width, making the hamburger button float without visual context.

## Design vs Implementation

| Property | Design | Implementation |
|----------|--------|---------------|
| Background | `#1A1D27` (`$bg-surface`) | transparent (none) |
| Width | full viewport width | not set |
| Bottom border | `1px solid #2E3142` | none |

## Affected File

- `src/ArchQ.Web/src/app/app.component.scss:163-172` — `.app-toolbar` in `@media (max-width: 1024px)`

## Recommended Fix

Add `width: 100%`, `background-color: #1A1D27`, and `border-bottom: 1px solid #2E3142` to the `.app-toolbar` media query rule.
