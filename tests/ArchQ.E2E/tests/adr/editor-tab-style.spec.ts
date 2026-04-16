import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-054: Mobile editor tabs use underline style', () => {

  test('Active tab should not have filled background', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const activeBlock = scss.match(/\.tab-btn\.active\s*\{[^}]*\}/s)?.[0] || '';

    // Design uses underlined text tab, not filled button
    // Active tab should have accent text color, not bg fill
    expect(activeBlock).not.toMatch(/background-color:\s*#0062FF/i);
    expect(activeBlock).toMatch(/color:\s*#0062FF/i);
  });
});
