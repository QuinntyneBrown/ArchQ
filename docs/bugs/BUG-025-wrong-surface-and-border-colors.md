# BUG-025: Wrong surface background and border colors throughout the app

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 2 - Register page audit)

## Summary

The application uses incorrect surface background and border colors that differ from the design system specification in `ui-design.pen`. The colors are close but not matching, creating a subtle but systemic inconsistency.

## Color Mismatches

| Token | Design Value | Implementation Value | Usage |
|-------|-------------|---------------------|-------|
| `$bg-surface` | `#1A1D27` | `#1A1D2E` | Card backgrounds, panels, sidebar |
| `$border-default` | `#2E3142` | `#2A2D3E` | Card borders, dividers, table borders |

## Affected Areas

Every card, panel, and bordered element across the entire application:

- Login card, Register card, Verify email card
- ADR list card, ADR cards (mobile)
- ADR editor panels (sidebar, preview, toolbar)
- Org switcher dropdown
- Toast notifications
- Tenant settings pages
- Table borders, divider lines

## Affected Files (14 occurrences of wrong bg, 28 of wrong border)

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss`
- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss`
- `src/ArchQ.Web/src/app/features/auth/verify-email/verify-email.component.scss`
- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss`
- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss`
- `src/ArchQ.Web/src/app/features/settings/tenant-create/tenant-create.component.scss`
- `src/ArchQ.Web/src/app/features/settings/tenant-detail/tenant-detail.component.scss`
- `src/ArchQ.Web/src/app/shared/components/org-switcher/org-switcher.component.scss`
- `src/ArchQ.Web/src/app/shared/components/toast/toast.component.scss`
- `src/ArchQ.Web/src/app/app.component.scss`

## Recommended Fix

Replace all instances of `#1a1d2e` with `#1A1D27` and `#2a2d3e` with `#2E3142` across all SCSS files.
