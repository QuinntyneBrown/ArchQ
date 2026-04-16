import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-055: Editor text uses $text-primary', () => {

  test('Title input and markdown headings should use #F0F1F5, not #ffffff', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const titleBlock = scss.match(/\.title-input\s*\{[^}]*\}/s)?.[0] || '';
    const mdHeadingLine = scss.match(/\.markdown-body h1.*\{[^}]*\}/s)?.[0] || '';

    // Title input should use $text-primary
    expect(titleBlock).toMatch(/color:\s*#F0F1F5/i);
    expect(titleBlock).not.toMatch(/color:\s*#ffffff/i);

    // Markdown headings should use $text-primary
    expect(mdHeadingLine).toMatch(/#F0F1F5/i);
    expect(mdHeadingLine).not.toMatch(/#ffffff/i);
  });
});
