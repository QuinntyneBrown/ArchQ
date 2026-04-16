# BUG-076: Mobile tabs container has extra padding and gap not in design

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - Mobile ADR Editor tabs)

## Summary

`.mobile-tabs` has `padding: 0.5rem 1.5rem` (8px 24px) and `gap: 0.5rem` (8px), but the design `meTabs` shows no container padding and no gap — the tabs fill the full width edge-to-edge with their own internal padding of 10px vertical.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:148` — `.mobile-tabs`

## Recommended Fix

Remove the padding and gap from `.mobile-tabs`:
- Change `padding: 0.5rem 1.5rem` to `padding: 0`
- Change `gap: 0.5rem` to `gap: 0`
