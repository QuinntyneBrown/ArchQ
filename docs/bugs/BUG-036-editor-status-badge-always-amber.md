# BUG-036: Editor status badge always shows amber regardless of actual status

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 12 — editor status audit)

## Summary

The ADR editor page status badge is hardcoded to amber (#F59E0B) for all statuses. When viewing an "Approved" ADR, the badge text correctly reads "approved" but the color remains amber instead of switching to emerald green (#10B981). This is confusing because amber means "Draft" in the design system.

## Steps to Reproduce

1. Approve an ADR through the workflow
2. Navigate to the editor for the approved ADR
3. Observe the status badge color — it's amber instead of green

## Root Cause

The editor template (`adr-editor.component.html:18`) renders the badge with a static `.status-badge` class that has hardcoded amber colors in the SCSS. Unlike the ADR list component which uses dynamic `[ngClass]="getStatusClass(adr.status)"`, the editor has no dynamic class binding for status colors.

## Affected Files

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.html:18`
- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss:45-53`
- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.ts`

## Recommended Fix

1. Add a `getStatusClass()` method to the editor component
2. Change template to `<span class="status-badge" [ngClass]="getStatusClass(existingAdr()?.status || 'draft')">`
3. Add status-specific CSS classes to the editor SCSS
