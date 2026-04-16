# BUG-045: Table header row missing $bg-elevated background

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 22 - Table header background audit)

## Summary

The ADR table header row (`th` cells) has no background color, inheriting the card surface color `#1A1D27`. The design specifies `fill: #242736` (`$bg-elevated`) for the table header row, creating visual separation between the header labels and data rows.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:130-137` — `.adr-table th`

## Recommended Fix

Add `background-color: #242736` to `.adr-table th`.
