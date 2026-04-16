# BUG-031: Input and button border-radius is 6px instead of $radius-sm 4px

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 8 - Border radius audit)

## Summary

Input fields, buttons, selects, and other interactive elements use `border-radius: 0.375rem` (6px) throughout the application, but the design system specifies `$radius-sm` = 4px for these elements. The Input/Default and Button/Primary components both have `cornerRadius: 4`.

## Design System Radius Tokens

| Token | Value | Used For |
|-------|-------|----------|
| `$radius-sm` | 4px | Inputs, buttons, badges, small elements |
| `$radius-md` | 8px | Cards, dropdowns |
| `$radius-lg` | 12px | Modal, login/register card |

## Mismatch

| Element | Design | Implementation |
|---------|--------|---------------|
| Input border-radius | 4px (`$radius-sm`) | 6px (0.375rem) |
| Button border-radius | 4px (`$radius-sm`) | 6px (0.375rem) |
| Select border-radius | 4px (`$radius-sm`) | 6px (0.375rem) |

## Scope

31 occurrences of `border-radius: 0.375rem` across 9 SCSS files.

## Recommended Fix

Replace `border-radius: 0.375rem` with `border-radius: 0.25rem` across all SCSS files.
