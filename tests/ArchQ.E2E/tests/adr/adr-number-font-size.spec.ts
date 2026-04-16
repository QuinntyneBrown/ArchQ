import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-063: ADR number link font-size should be 0.8125rem (13px)', () => {

  test('adr-number-link CSS should use 0.8125rem, not 0.75rem', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.adr-number-link\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toContain('font-size: 0.8125rem');
    expect(block).not.toMatch(/font-size:\s*0\.75rem/);
  });
});
