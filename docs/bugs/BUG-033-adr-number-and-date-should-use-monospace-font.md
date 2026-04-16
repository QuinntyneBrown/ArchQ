# BUG-033: ADR numbers and dates should use monospace font

**Severity:** Low  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 9 — typography audit)

## Summary

The design system uses `Geist Mono` (a monospace font) for ADR numbers (e.g., "ADR-001") and dates (e.g., "Apr 10, 2026") throughout the ADR list — both in the desktop table and mobile cards. The actual implementation renders these elements in Inter (sans-serif), losing the visual distinction that monospace provides for structured data.

## Design Spec (from ui-design.pen)

| Element | Design Font | Design Size | Actual Font | Actual Size |
|---------|------------|-------------|-------------|-------------|
| ADR Number (table) | Geist Mono | 12px | Inter | 14px |
| ADR Number (mobile card) | Geist Mono 500 | 12px | Inter 600 | 14px |
| Date (mobile card) | Geist Mono | 11px | Inter | 12px |

## Affected Files

- `src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss` — `.adr-number-link`, `.date-cell`, `.card-number`, `.card-date`

## Recommended Fix

Add a monospace font-family to ADR number and date elements:

```scss
.adr-number-link, .card-number {
  font-family: 'Geist Mono', 'Fira Code', 'JetBrains Mono', monospace;
}
.date-cell, .card-date {
  font-family: 'Geist Mono', 'Fira Code', 'JetBrains Mono', monospace;
}
```

And import Geist Mono in `styles.scss`:
```scss
@import url('https://fonts.googleapis.com/css2?family=Geist+Mono:wght@400;500&display=swap');
```
