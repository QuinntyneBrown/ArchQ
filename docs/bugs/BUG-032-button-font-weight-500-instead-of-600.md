# BUG-032: Button font-weight is 500 (medium) instead of 600 (semibold)

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 9 - Button typography audit)

## Summary

Primary action buttons (`.btn-primary`, `.btn-save`) use `font-weight: 500` but the design system's Button/Primary component specifies `fontWeight: 600` (semibold). This makes button text appear slightly thinner than designed.

## Design vs Implementation

| Element | Design | Implementation |
|---------|--------|---------------|
| Button/Primary font-weight | 600 (semibold) | 500 (medium) |

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss` — `.btn-primary`
- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss` — `.btn-primary`
- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.btn-primary`
- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss` — `.btn-save`
- `src/ArchQ.Web/src/app/features/settings/tenant-create/tenant-create.component.scss` — `.btn-primary`
- `src/ArchQ.Web/src/app/features/settings/tenant-detail/tenant-detail.component.scss` — `.btn-primary`

## Recommended Fix

Change `font-weight: 500` to `font-weight: 600` in all `.btn-primary` and `.btn-save` rules.
