# BUG-034: Status badges, nav labels, and divider text missing Geist Mono font

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 10 — typography audit)

## Summary

The design system uses `Geist Mono` (monospace) for several UI elements beyond ADR numbers: status badges, the "NAVIGATION" section label, and the "OR" divider on the login page. All currently render in Inter (sans-serif).

## Affected Elements

| Element | Location | Design Font | Actual Font |
|---------|----------|-------------|-------------|
| Status badges ("Draft", "Approved") | ADR list & editor | Geist Mono 12px 500 | Inter 12px 500 |
| "NAVIGATION" label | Sidebar | Geist Mono 11px 500 | Inter 11px 600 |
| "OR" divider text | Login page | Geist Mono 11px | Inter 11px |

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.status-badge`
- `src/ArchQ.Web/src/app/app.component.scss` — `.nav-section-label`
- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss` — `.divider-text`

## Recommended Fix

Add `font-family: 'Geist Mono', monospace;` to each of these classes.
