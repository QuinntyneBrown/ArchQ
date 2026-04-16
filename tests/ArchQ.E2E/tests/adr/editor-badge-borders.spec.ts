import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-056: Editor badges should not have borders', () => {

  test('Editor status badge CSS should not contain border declarations', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const draftBlock = scss.match(/\.status-draft\s*\{[^}]*\}/s)?.[0] || '';
    const reviewBlock = scss.match(/\.status-in-review\s*\{[^}]*\}/s)?.[0] || '';

    expect(draftBlock).not.toMatch(/border:/);
    expect(reviewBlock).not.toMatch(/border:/);
  });
});
