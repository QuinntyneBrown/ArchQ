# BUG-009: Markdown preview panel never renders content

**Severity:** Medium  
**Status:** Open  
**Date Discovered:** 2026-04-16  
**Discovered By:** Automated monkey testing (Iteration 9)

## Summary

The ADR editor's "Preview" panel shows a header but never renders the markdown content. The design spec shows a side-by-side editor with live preview rendering headings and paragraphs, but the live UI preview is always empty.

## Root Cause

In `adr-editor.component.ts`, `renderedHtml` is defined as a `computed()` signal that reads `this.content`:

```typescript
readonly renderedHtml = computed(() => {
    const raw = marked.parse(this.content || '', { async: false }) as string;
    return DOMPurify.sanitize(raw);
});
```

But `content` is declared as a plain string property (`content = ''`), not a signal. Angular's `computed()` only tracks signal dependencies — since `content` is not a signal, the computed never re-evaluates after initial creation.

## Affected File

- `src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.ts:22,35-42`

## Recommended Fix

Convert `content` to a writable signal (`content = signal('')`) and update all references to use `content()` for reading and `content.set(value)` for writing.
