# BUG-024: Wrong accent color used throughout the application

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (UI audit against ui-design.pen)

## Summary

The entire application uses `#3B82F6` (Tailwind blue-500 / design system `$info` token) as the primary interactive color for buttons, links, focus rings, and active states. The design system specifies `#0062FF` (`$accent-primary`) for these elements. This creates a noticeably different, less saturated blue compared to the design.

## Affected Areas

Every screen and component that uses blue as an interactive color:

- **Login page:** Sign In button, Forgot password link, Sign up link, input focus border
- **Register page:** Create Account button, Sign in link, input focus border
- **ADR List:** New ADR button, ADR number links, active filter pill, search focus border
- **ADR Detail/Editor:** Action buttons, links
- **Global styles:** Anchor tag default color

## Design System Reference

| Token | Design Value | Implementation Value | Purpose |
|-------|-------------|---------------------|---------|
| `$accent-primary` | `#0062FF` | `#3B82F6` (wrong) | Buttons, links, focus |
| `$accent-primary-hover` | `#0052D9` | `#1D4ED8` (wrong) | Button hover states |
| `$info` | `#3B82F6` | `#3B82F6` | Info badges only |

## Affected Files

- `src/ArchQ.Web/src/styles.scss` (global anchor color)
- `src/ArchQ.Web/src/app/features/auth/login/login.component.scss`
- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss`
- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss`
- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss`

## Recommended Fix

Replace all instances of `#3b82f6` with `#0062FF` and `#1d4ed8` / `#2563eb` hover colors with `#0052D9` across all SCSS files. Consider introducing CSS custom properties (`--accent-primary: #0062FF`) to prevent future drift.
