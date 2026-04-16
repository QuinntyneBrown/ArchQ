# BUG-066: Nav item font-weight missing — should be 500 (inactive) / 600 (active)

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - Sidebar navigation)

## Summary

`.nav-item` in the sidebar has no `font-weight` set, so it defaults to `normal` (400). The design specifies `500` for inactive nav items and `600` for the active item (`.nav-active`).

## Affected File

- `src/ArchQ.Web/src/app/app.component.scss:62` — `.nav-item`
- `src/ArchQ.Web/src/app/app.component.scss:78` — `&.nav-active`

## Recommended Fix

1. Add `font-weight: 500` to `.nav-item`
2. Add `font-weight: 600` to `&.nav-active`
