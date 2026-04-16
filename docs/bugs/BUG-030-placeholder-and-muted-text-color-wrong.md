# BUG-030: Placeholder and muted text color is #6B7280 instead of $text-disabled #5C5F6E

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 7 - Placeholder color audit)

## Summary

Input placeholders, search icons, table header text, divider text, card dates, and other muted UI elements use `#6B7280` instead of the design system's `$text-disabled` token (`#5C5F6E`). The current color is slightly brighter than intended, reducing contrast between disabled/muted and secondary text levels.

## Color Mismatch

| Token | Design Value | Implementation Value |
|-------|-------------|---------------------|
| `$text-disabled` | `#5C5F6E` | `#6B7280` |

## Scope

17 occurrences across 7 SCSS files, used for:
- Input `::placeholder` text
- Search icon color
- Table header text (`th`)
- Preview pane header labels
- Card date text
- Divider text ("OR")
- Pagination total count

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss`
- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss`
- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss`
- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss`
- `src/ArchQ.Web/src/app/features/settings/tenant-create/tenant-create.component.scss`
- `src/ArchQ.Web/src/app/features/settings/tenant-detail/tenant-detail.component.scss`
- `src/ArchQ.Web/src/app/app.component.scss`

## Recommended Fix

Replace all `#6b7280` with `#5C5F6E` across all SCSS files.
