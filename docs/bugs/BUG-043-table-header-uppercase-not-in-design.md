# BUG-043: Table header text is uppercase but design uses title case

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 20 - Desktop table audit)

## Summary

The ADR table header cells use `text-transform: uppercase` making them display as "NUMBER", "TITLE", "STATUS", etc. The design shows these headers in title case: "Number", "Title", "Status", "Author", "Modified".

## Design vs Implementation

| Header | Design | Implementation |
|--------|--------|---------------|
| Column 1 | "Number" | "NUMBER" (uppercase) |
| Column 2 | "Title" | "TITLE" (uppercase) |
| Column 3 | "Status" | "STATUS" (uppercase) |
| Column 4 | "Author" | "AUTHOR" (uppercase) |
| Column 5 | "Modified" | "MODIFIED" (uppercase) |

## Affected File

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:135` — `text-transform: uppercase`

## Recommended Fix

Remove `text-transform: uppercase` from `.adr-table th`.
