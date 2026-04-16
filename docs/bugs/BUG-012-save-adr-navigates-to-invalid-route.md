# BUG-012: Saving ADR navigates to non-existent route

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 12)

## Summary

After saving a new or updated ADR, the editor navigates to `/adrs/{id}` which is not a valid route. The Angular router has no match for this path, causing the navigation to silently fail and the user remains on the current page (e.g., `/adrs/new`). The correct route is `/adrs/{id}/edit`.

## Root Cause

In `adr-editor.component.ts`:
- Line 120 (after update): `this.router.navigate(['/adrs', resp.id])`
- Line 135 (after create): `this.router.navigate(['/adrs', adr.id])`

Both navigate to `/adrs/{id}` but only `/adrs/:id/edit` is defined in `app.routes.ts`.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.ts:120,135`

## Recommended Fix

Change both navigations to `this.router.navigate(['/adrs', id, 'edit'])`.
