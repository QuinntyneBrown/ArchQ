# BUG-016: Search input missing magnifying glass icon

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 16)

## Summary

The search input on the ADR list page is a plain text input. The design spec (Ae0g2.png) shows a magnifying glass (search) icon inside the input field on the left side, providing a visual cue that the field is for searching.

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.html:13-21`
- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss:55-67`

## Recommended Fix

Wrap the search input in a container, add an SVG search icon positioned absolutely on the left, and add left padding to the input.
