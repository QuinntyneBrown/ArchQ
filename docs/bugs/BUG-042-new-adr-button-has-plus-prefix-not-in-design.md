# BUG-042: "New ADR" button has "+" prefix not present in the design

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 19 - ADR list button audit)

## Summary

The desktop "New ADR" button on the ADR list page shows `+ New ADR` but the design specifies just `New ADR` (without the plus prefix). The design's Button/Primary component for this context has `content: "New ADR"`.

## Design vs Implementation

| Element | Design | Implementation |
|---------|--------|---------------|
| New ADR button text | "New ADR" | "+ New ADR" |

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.html:9` — button text

## Recommended Fix

Change `+ New ADR` to `New ADR` in the button text.
