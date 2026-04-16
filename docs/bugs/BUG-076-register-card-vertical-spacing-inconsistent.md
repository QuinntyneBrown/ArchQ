# BUG-076: Register card vertical spacing does not match design spec

## Summary
The Register Card's vertical spacing between child sections is inconsistent with the design spec, which specifies a uniform `gap: 24px` between all top-level children.

## Actual Behavior
| Gap                    | Actual | Expected |
|------------------------|--------|----------|
| Brand → Subtitle       | 8px    | 24px     |
| Subtitle → Form        | 24px   | 24px     |
| Last form → Button     | 40px   | 24px     |
| Button → Sign-in link  | 20px   | 24px     |

## Expected Behavior
All gaps between top-level child sections of the Register Card should be 24px, matching the design spec's `gap: 24` property.

## Root Cause
The `.register-card` uses individual margins on children instead of flexbox `gap`. The `.brand` has `margin-bottom: 0.5rem` (8px) which is too small, the `.btn-primary` has `margin-top: 1.5rem` (24px) that doesn't collapse with `.form-group`'s `margin-bottom: 1rem` (16px) because buttons are `inline-block`, and `.sign-in-prompt` has `margin-top: 1.25rem` (20px) instead of 24px.

## Fix
Convert `.register-card` to use `display: flex; flex-direction: column; gap: 1.5rem` and remove individual margins from children.

## Severity
Low — visual spacing inconsistency

## Steps to Reproduce
1. Navigate to http://localhost:4200/register
2. Measure vertical gaps between brand, subtitle, form, button, and sign-in sections
3. Observe gaps are 8px, 24px, 40px, 20px instead of uniform 24px
