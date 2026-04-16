# BUG-028: Wrong elevated background and input border colors throughout the app

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 5 - Input field audit)

## Summary

Input fields, select dropdowns, and other elevated interactive elements use `#252836` for their background color instead of the design system's `$bg-elevated` (`#242736`). Similarly, input borders use `#3A3F54` instead of `$border-default` (`#2E3142`). This is a companion to BUG-025 which fixed the surface-level colors; these are the elevated-level colors for interactive elements.

## Color Mismatches

| Token | Design Value | Implementation Value | Usage |
|-------|-------------|---------------------|-------|
| `$bg-elevated` | `#242736` | `#252836` | Input fields, selects, search bars, dropdowns |
| `$border-default` | `#2E3142` | `#3A3F54` | Input borders, select borders, interactive element borders |

## Scope

- 15 occurrences of `#252836` across 7 files
- 16 occurrences of `#3A3F54` across 8 files

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss`
- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss`
- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss`
- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss`
- `src/ArchQ.Web/src/app/features/settings/tenant-create/tenant-create.component.scss`
- `src/ArchQ.Web/src/app/features/settings/tenant-detail/tenant-detail.component.scss`
- `src/ArchQ.Web/src/app/shared/components/org-switcher/org-switcher.component.scss`
- `src/ArchQ.Web/src/app/app.component.scss`

## Recommended Fix

Replace all `#252836` with `#242736` and all `#3a3f54` with `#2E3142` across all SCSS files.
