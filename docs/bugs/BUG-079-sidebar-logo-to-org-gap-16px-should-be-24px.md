# BUG-079: Sidebar logo-to-org-switcher gap is 16px, should be 24px

## Summary
The vertical gap between the sidebar logo row and the org switcher is 16px, but the design spec requires 24px (16px logoRow bottom padding + 8px sidebar gap).

## Actual Behavior
- Gap from logo row bottom to org switcher top: 16px

## Expected Behavior
- Gap should be 24px per design spec (sideLogoRow padding-bottom: 16 + sidebar gap: 8)

## Root Cause
`.sidebar-header` has `margin-bottom: 1rem` (16px) instead of `1.5rem` (24px).

## Fix
Change `.sidebar-header` margin-bottom from `1rem` to `1.5rem`.

## Severity
Low — minor spacing inconsistency in sidebar

## Steps to Reproduce
1. Log in and observe the sidebar on any page
2. Measure the vertical gap between "ArchQ" logo row and the org switcher
3. Gap is 16px, design spec requires 24px
