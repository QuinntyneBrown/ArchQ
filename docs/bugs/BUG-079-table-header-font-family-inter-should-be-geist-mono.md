# BUG-079: Table header font-family inherits Inter but should be Geist Mono

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Design audit - ADR List table header)

## Summary

`.adr-table th` does not set `font-family`, so it inherits `Inter` from the global styles. The design specifies table header text (`Number`, `Title`, `Status`, `Author`, `Modified`) in `Geist Mono` monospace font.

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:131` — `.adr-table th`

## Recommended Fix

Add `font-family: 'Geist Mono', 'Fira Code', 'JetBrains Mono', monospace;` to `.adr-table th`.
