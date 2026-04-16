# BUG-085: Editor back arrow icon is 20px on desktop, should be 16px

## Summary
The editor top-bar back arrow SVG is 20x20px on all viewports. The desktop design spec requires 16x16px, while mobile correctly uses 20x20px.

## Actual Behavior
- Back arrow icon is 20x20px on desktop and mobile

## Expected Behavior
- Desktop: 16x16px (per design spec `edBackIcon`: 16x16)
- Mobile (≤768px): 20x20px (per design spec `meBackIcon`: 20x20)

## Root Cause
The SVG element in `adr-editor.component.html` has hardcoded `width="20" height="20"` attributes with no responsive override.

## Fix
Change the SVG to use CSS sizing: add a class and set desktop size to 16px, with a mobile override to 20px.

## Severity
Low — 4px icon size difference on desktop

## Steps to Reproduce
1. Navigate to ADR editor on desktop viewport
2. Inspect the back arrow SVG icon
3. Icon is 20x20px, design spec requires 16x16px on desktop
