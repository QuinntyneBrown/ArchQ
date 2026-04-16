import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-078: Toolbar icon size should be 0.875rem (14px)', () => {

  test('toolbar-btn font-size should be 0.875rem, not 0.8125rem', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.toolbar-btn\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toContain('font-size: 0.875rem');
    expect(block).not.toMatch(/font-size:\s*0\.8125rem/);
  });
});
