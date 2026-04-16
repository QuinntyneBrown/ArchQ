# BUG-083: ADR table title cell font-weight is 400, should be 500

## Summary
The ADR table title text uses the default font-weight of 400 (normal), but the design spec requires font-weight 500 (medium).

## Actual Behavior
- Title cell text has font-weight: normal (400)

## Expected Behavior
- Title cell text should have font-weight: 500
- Per design spec: title text uses `fontWeight: "500"`, Inter, $font-sm (14px)

## Root Cause
`.title-cell` in `adr-list.component.scss` does not set a `font-weight` property, so it inherits the default `normal` (400) from the `td` element.

## Fix
Add `font-weight: 500;` to `.title-cell`.

## Severity
Low — subtle typography weight difference

## Steps to Reproduce
1. Log in and view the ADR list with records
2. Inspect the title column text weight
3. Title text is 400 weight, design spec requires 500
