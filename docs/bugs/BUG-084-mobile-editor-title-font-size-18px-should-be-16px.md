# BUG-084: Mobile editor title font-size is 18px, should be 16px

## Summary
The ADR editor top-bar title uses 18px font-size on all viewports, but the mobile design spec requires 16px ($font-base). Desktop correctly uses 18px ($font-lg).

## Actual Behavior
- Mobile (≤768px): `.top-bar-title` font-size is 18px (1.125rem)

## Expected Behavior
- Mobile: font-size should be 16px (1rem) per mobile editor header design (`$font-base`)
- Desktop: font-size 18px (1.125rem) per desktop editor design (`$font-lg`)

## Root Cause
`.top-bar-title` has `font-size: 1.125rem` with no mobile-specific override in the `@media (max-width: 768px)` block.

## Fix
Add `font-size: 1rem;` to `.top-bar-title` inside the existing mobile media query.

## Severity
Low — 2px font-size difference on mobile only

## Steps to Reproduce
1. Navigate to ADR editor (/adrs/new) at mobile viewport (375px)
2. Inspect the top-bar-title element
3. Font-size is 18px, should be 16px
