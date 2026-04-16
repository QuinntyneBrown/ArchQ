import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-053: Editor title font-size and color', () => {

  test('Editor title CSS should use 1.125rem (18px) and #F0F1F5', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Get the first .top-bar-title block (not the media query override)
    const block = scss.match(/^\.top-bar-title\s*\{[^}]*\}/m)?.[0] || '';

    expect(block).toMatch(/font-size:\s*1\.125rem/);
    expect(block).toMatch(/#F0F1F5/i);
  });
});
