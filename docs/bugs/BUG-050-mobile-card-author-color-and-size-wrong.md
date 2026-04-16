# BUG-050: Mobile card author text uses wrong color and font-size

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 27 - Mobile card author audit)

## Summary

The `.card-author` text on mobile ADR cards uses `#9CA3AF` (`$text-secondary`) and `13px` but the design specifies `#5C5F6E` (`$text-disabled`) and `12px`. The author name should be dimmer than secondary text to create visual hierarchy.

## Design vs Implementation

| Property | Design | Implementation |
|----------|--------|---------------|
| Color | `#5C5F6E` (`$text-disabled`) | `#9CA3AF` (`$text-secondary`) |
| Font-size | 12px | 13px (0.8125rem) |

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.card-author`

## Recommended Fix

Change `.card-author` color to `#5C5F6E` and font-size to `0.75rem`.
