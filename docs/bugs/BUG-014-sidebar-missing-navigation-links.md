# BUG-014: Sidebar missing navigation links per design spec

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 14)

## Summary

The sidebar only contains the "ArchQ" logo text and an org switcher. The design spec (Ae0g2.png) shows a "NAVIGATION" section with links: "ADR Records" (with active highlight), "Search", and other items. The sidebar is missing all navigation links, providing no way for users to navigate between sections.

## Design Spec (Ae0g2.png)

- "NAVIGATION" section header
- "ADR Records" link (active state: blue background + white text)
- "Search" link
- Additional nav items (Templates, Team Members, Settings)

## Actual

- No navigation section
- No links at all
- Only: logo text + org switcher

## Affected File

- `src/ArchQ.Web/src/app/app.component.html` — sidebar `<aside>` has no nav links

## Recommended Fix

Add a navigation section to the sidebar with at least an "ADR Records" link (routerLink="/adrs") with active state styling.
