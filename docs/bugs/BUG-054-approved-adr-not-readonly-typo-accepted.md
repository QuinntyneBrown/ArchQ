# BUG-054: Approved ADRs not read-only due to "accepted" typo in status check

**Severity:** High  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 30 — functional edge-case testing)

## Summary

Approved ADRs can be edited because the readOnly check uses 'accepted' instead of 'approved'.

## Affected Files

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.ts:35`
