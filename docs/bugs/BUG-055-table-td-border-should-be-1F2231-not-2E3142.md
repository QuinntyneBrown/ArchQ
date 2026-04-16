# BUG-055: Table data row border should be #1F2231 (subtle), not #2E3142

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 32 — design re-verification)

## Summary

BUG-047 incorrectly changed table `td` border from `#1F2231` to `#2E3142` assuming it should match the `th`. However, the design spec intentionally uses different colors: `#242736` header background for visual separation, and subtler `#1F2231` borders between data rows. The `#2E3142` border token is for card/container borders, not internal row dividers.

## Design Spec (ADR Table, node Zqpvy)

- Table Header: `fill: #242736` (elevated bg, no explicit border needed)
- Data rows: `stroke: { fill: "#1F2231", thickness: { bottom: 1 } }`

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:143`
- `tests/ArchQ.E2E/tests/adr/adr-table-td-border-color.spec.ts` (test needs update)

## Recommended Fix

Change `.adr-table td { border-bottom: 1px solid #2E3142; }` back to `#1F2231`.
