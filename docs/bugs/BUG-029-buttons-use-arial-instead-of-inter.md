# BUG-029: All buttons use Arial instead of Inter font

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 5 — font audit)

## Summary

Every `<button>` element in the application renders in Arial (browser default) instead of the Inter font specified by the design system. This affects all buttons: Sign In, Create Account, Save Draft, Logout, Org Switcher, filter pills, and toolbar buttons.

## Root Cause

The global stylesheet (`styles.scss`) sets `font-family: 'Inter', ...` on `html, body`, but `<button>` elements do not inherit `font-family` by default in CSS — they use the browser's built-in form control styling (typically Arial or system-ui).

## Affected Files

- `src/ArchQ.Web/src/styles.scss` — missing `font-family: inherit` for form elements

## Recommended Fix

Add a reset rule for form elements in the global stylesheet:

```scss
button, input, select, textarea {
  font-family: inherit;
}
```
