# BUG-043: ADR list card border-radius is 12px instead of 8px

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 19 — radius audit)

## Summary

The `.adr-list-card` uses `border-radius: 0.75rem` (12px) but the design system uses `$radius-md` (8px / `cornerRadius: 8`) for content cards within the app. The 12px radius (`$radius-lg`) is reserved for standalone auth cards (Login, Register). Content cards like the ADR table and mobile ADR cards should use 8px.

## Design System Radius Tokens

| Token | Value | Usage |
|-------|-------|-------|
| `$radius-lg` | 12px | Auth cards (Login, Register) |
| `$radius-md` | 8px | Content cards (ADR table, mobile cards) |
| `$radius-sm` | 4px | Inputs, buttons, nav items |

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:12`

## Recommended Fix

Change `.adr-list-card { border-radius: 0.75rem; }` to `border-radius: 0.5rem;` (8px).
