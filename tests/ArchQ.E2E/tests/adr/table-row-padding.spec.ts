import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-064: Table row vertical padding should be 0.875rem (14px)', () => {

  test('adr-table td should use 0.875rem 1rem padding', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.adr-table\s+td\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toContain('padding: 0.875rem 1rem');
    expect(block).not.toMatch(/padding:\s*0\.75rem\s+1rem/);
  });
});
