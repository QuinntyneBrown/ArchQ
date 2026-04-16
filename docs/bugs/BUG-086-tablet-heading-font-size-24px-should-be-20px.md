# BUG-086: Tablet ADR list heading font-size is 24px, should be 20px

## Summary
The ADR list page heading "Architecture Decision Records" is 24px at tablet viewport (768px), but the tablet design spec requires 20px. The mobile override (20px) only applies at ≤575px, missing the tablet range.

## Actual Behavior
- Tablet (576-1024px): heading font-size is 24px (uses desktop value)

## Expected Behavior
- Tablet: heading font-size should be 20px (per tablet design `tabPageTitle: fontSize 20`)
- Desktop (>1024px): 24px ($font-2xl)
- Mobile (≤575px): 20px (already correct)

## Root Cause
The `.heading` font-size override in `@media (max-width: 575px)` only applies to mobile. The tablet range (576-1024px) inherits the desktop 24px.

## Fix
Extend the heading font-size override to apply at `max-width: 1024px` instead of `max-width: 575px`.

## Severity
Low — 4px font-size difference on tablet only

## Steps to Reproduce
1. Navigate to ADR list at tablet viewport (768px)
2. Inspect the heading "Architecture Decision Records"
3. Font-size is 24px, tablet design spec requires 20px
