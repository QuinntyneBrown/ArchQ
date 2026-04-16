# BUG-027: Password strength bar visible on register page when password is empty

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** User report + automated monkey testing (Iteration 3)

## Summary

The register page shows a visible 4px-tall horizontal bar between the Password and Organization Name fields, even when the password field is empty. This element is not present in the design spec (`docs/ui-design.pen`, node `Liwyv` — Register Card). The bar creates a visual break that makes the form look broken.

## Steps to Reproduce

1. Navigate to `http://localhost:4200/register`
2. Observe the area between the Password input and Organization Name label
3. A thin horizontal line/bar is visible

## Expected Result

Per the design spec, the four form fields (Full Name, Email, Password, Organization Name) should flow vertically with consistent 16px gaps and no dividers or extra elements between them.

## Actual Result

A 4px-tall dark bar (`background-color: #252836`) renders between the Password field and Organization Name field. Although subtle, the contrast against the card background (`#1A1D27`) makes it clearly visible as an unintended divider.

## Root Cause

`register.component.html` lines 65-71 contain a `<div class="password-strength">` element that is always rendered in the DOM with a fixed 4px height and background color, regardless of whether the password field has input.

```html
<div class="password-strength" data-testid="password-strength">
  <div class="password-strength-bar" [style.width.%]="passwordStrength()" ...></div>
</div>
```

The SCSS (`register.component.scss:68-74`) gives it a constant height and visible background:

```scss
.password-strength {
  height: 4px;
  background-color: #252836;
  margin-top: 0.5rem;
}
```

## Affected Files

- `src/ArchQ.Web/src/app/features/auth/register/register.component.scss:68-74`

## Recommended Fix

Hide the password strength bar when the password is empty by making the container transparent and only showing it when there is input:

```scss
.password-strength {
  height: 4px;
  background-color: transparent;
  border-radius: 2px;
  margin-top: 0.5rem;
  overflow: hidden;
}
```

Or conditionally render it in the template with `@if (password())`.
