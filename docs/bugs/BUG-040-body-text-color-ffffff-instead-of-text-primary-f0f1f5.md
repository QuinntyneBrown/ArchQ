# BUG-040: Global body text color is #FFFFFF instead of $text-primary #F0F1F5

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 17 - Text primary color audit)

## Summary

The global body text color in `styles.scss` is set to `#ffffff` (pure white) but the design system specifies `$text-primary: #F0F1F5` (a slightly warm off-white). This affects all text that inherits from the body, including headings, logo text, and input text across the entire app.

Note: Button text on accent-colored backgrounds should remain `#FFFFFF` (`$text-inverse`), not `$text-primary`.

## Design Tokens

| Token | Value | Usage |
|-------|-------|-------|
| `$text-primary` | `#F0F1F5` | Headings, body text, input text on dark backgrounds |
| `$text-inverse` | `#FFFFFF` | Button text on accent/colored backgrounds |

## Key Affected Location

- `src/ArchQ.Web/src/styles.scss:14` — `color: #ffffff` should be `color: #F0F1F5`

## Recommended Fix

Change the global body `color` from `#ffffff` to `#F0F1F5` in `styles.scss`. This propagates to all elements that inherit text color, fixing headings, logo text, and input text automatically.
