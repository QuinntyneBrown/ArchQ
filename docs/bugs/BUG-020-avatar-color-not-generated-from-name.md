# BUG-020: Author avatars use flat gray instead of per-user colors

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 21)

## Summary

All author avatars in the ADR list use a static gray background (#374151). The design spec (Ae0g2.png, TjWsF.png) shows each user with a distinct colored avatar: purple for QB, blue for BK, green for CM, etc. Colors should be generated from the user's name to differentiate authors visually.

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:209` — hardcoded gray
- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.html:133,161` — no color binding
- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.ts` — needs color generation method

## Recommended Fix

Add a `getAvatarColor(name)` method that hashes the name to one of several predefined colors, and bind it via `[style.background-color]` on the avatar span.
