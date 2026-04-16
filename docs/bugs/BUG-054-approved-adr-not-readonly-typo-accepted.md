# BUG-054: Approved ADRs not read-only due to "accepted" typo in status check

**Severity:** High  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 30 — functional edge-case testing)

## Summary

Approved ADRs can still be edited and saved because the editor's `readOnly` computed property checks for the status `'accepted'` instead of `'approved'`. This means:

1. No read-only banner is shown for approved ADRs
2. The Save Draft button remains enabled
3. Users can modify and overwrite approved ADR content

This defeats the purpose of the approval workflow — approved decisions should be immutable.

## Root Cause

In `adr-editor.component.ts:35`:
```typescript
const terminalStatuses = ['accepted', 'rejected', 'superseded', 'deprecated'];
```

The status `'accepted'` doesn't exist in the system. The correct value is `'approved'` (as stored by the workflow transition logic).

## Affected Files

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.ts:35`

## Recommended Fix

Change `'accepted'` to `'approved'`:
```typescript
const terminalStatuses = ['approved', 'rejected', 'superseded', 'deprecated'];
```
