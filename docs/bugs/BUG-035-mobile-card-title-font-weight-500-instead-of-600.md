# BUG-035: Mobile ADR card title font-weight is 500 instead of 600

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 12 - Mobile card audit)

## Summary

The ADR card title text on the mobile card list uses `font-weight: 500` (medium) but the design specifies `fontWeight: 600` (semibold). This makes card titles appear slightly thinner than designed, reducing their visual prominence against the card's secondary text.

## Design vs Implementation

| Property | Design | Implementation |
|----------|--------|---------------|
| Card title font-weight | 600 (semibold) | 500 (medium) |

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:260-265` — `.card-title`

## Recommended Fix

Change `.card-title` font-weight from `500` to `600`.
