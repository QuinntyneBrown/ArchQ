# BUG-029: Form label color is #D1D5DB instead of $text-secondary #9CA3AF

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 6 - Form label audit)

## Summary

Form input labels across the application use `#D1D5DB` for their text color, but the design system's `Input/Default` component specifies `#9CA3AF` (`$text-secondary`). This makes labels appear brighter than intended, reducing the visual hierarchy between labels and primary content.

## Design vs Implementation

| Element | Design | Implementation |
|---------|--------|---------------|
| Input label color | `#9CA3AF` (`$text-secondary`) | `#D1D5DB` |

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss:59`
- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss:48`
- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:103`
- `src/ArchQ.Web/src/app/features/settings/tenant-create/tenant-create.component.scss:24`

## Recommended Fix

Change the label `color` from `#d1d5db` to `#9CA3AF` in the `label` CSS rule in each affected file.
