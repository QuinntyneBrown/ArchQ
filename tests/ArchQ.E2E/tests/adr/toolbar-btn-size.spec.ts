import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-061: Toolbar button size should be 1.75rem (28px)', () => {

  test('toolbar-btn CSS should use 1.75rem, not 2rem', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.toolbar-btn\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toContain('width: 1.75rem');
    expect(block).toContain('height: 1.75rem');
    expect(block).not.toMatch(/width:\s*2rem/);
    expect(block).not.toMatch(/height:\s*2rem/);
  });
});
