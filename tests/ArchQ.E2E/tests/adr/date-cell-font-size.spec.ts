import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-049: Date cell font-size matches design', () => {

  test('Date cell CSS should use 0.75rem (12px), not 0.8125rem (13px)', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.date-cell\s*\{[^}]*\}/s)?.[0] || '';

    // Design specifies 12px for date text
    expect(block).toMatch(/font-size:\s*0\.75rem/);
    expect(block).not.toMatch(/font-size:\s*0\.8125rem/);
  });
});
