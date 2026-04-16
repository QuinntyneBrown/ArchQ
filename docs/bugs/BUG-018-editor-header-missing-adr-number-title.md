# BUG-018: Editor header shows generic "Edit ADR" instead of ADR number and title

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 18)

## Summary

The ADR editor page header shows "Edit ADR" in edit mode. The design spec (Huy4L.png) shows "Editing ADR-003: CQRS Pattern" — including the specific ADR number and a shortened title for context.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.html:17`

## Recommended Fix

Change the edit mode header from `'Edit ADR'` to `'Editing ' + existingAdr().adrNumber + ': ' + existingAdr().title`.
