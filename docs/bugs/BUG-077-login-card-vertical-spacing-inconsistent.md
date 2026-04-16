# BUG-077: Login card vertical spacing does not match design spec

## Summary
The Login Card's vertical spacing between child sections is inconsistent with the design spec, which specifies a uniform `gap: 32px` between all top-level children.

## Actual Behavior
| Gap                    | Actual | Expected |
|------------------------|--------|----------|
| Brand → Subtitle       | 8px    | 32px     |
| Subtitle → Form        | 24px   | 32px     |
| Last form → Button     | 44px   | 32px     |
| Button → Forgot link   | 16px   | 32px     |
| Forgot → Divider       | 24px   | 32px     |
| Divider → Sign-up link | 24px   | 32px     |

## Expected Behavior
All gaps between top-level child sections of the Login Card should be 32px, matching the design spec's `gap: 32` property.

## Root Cause
The `.login-card` uses individual margins on children instead of flexbox `gap`. Margins are inconsistent and don't collapse correctly with inline-block elements like buttons.

## Fix
Convert `.login-card` to use `display: flex; flex-direction: column; gap: 2rem` and remove conflicting individual margins from children.

## Severity
Low — visual spacing inconsistency

## Steps to Reproduce
1. Navigate to http://localhost:4200/login
2. Measure vertical gaps between brand, subtitle, form, button, forgot link, divider, and sign-up sections
3. Observe gaps range from 8px to 44px instead of uniform 32px
