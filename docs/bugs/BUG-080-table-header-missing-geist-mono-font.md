# BUG-080: ADR table header text missing Geist Mono font-family

## Summary
The ADR table header cells (th) do not specify a font-family, defaulting to Inter. The design spec requires Geist Mono for all table header text (Number, Title, Status, Author, Modified).

## Actual Behavior
- Table headers use inherited font (Inter)

## Expected Behavior
- Table headers should use `font-family: 'Geist Mono', 'Fira Code', 'JetBrains Mono', monospace`
- Per design spec: all 5 column headers use `fontFamily: "Geist Mono"`, fontSize: 12, fontWeight: 600

## Root Cause
`.adr-table th` in `adr-list.component.scss` does not include a `font-family` declaration.

## Fix
Add `font-family: 'Geist Mono', 'Fira Code', 'JetBrains Mono', monospace;` to `.adr-table th`.

## Severity
Low — typography inconsistency in table headers

## Steps to Reproduce
1. Log in and navigate to the ADR list with at least one ADR
2. Inspect the table header cells (Number, Title, Status, Author, Modified)
3. Observe the font-family is Inter instead of Geist Mono
