import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-043: Table header text case matches design', () => {

  test('Table header CSS should not use text-transform: uppercase', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const thBlock = scss.match(/\.adr-table th\s*\{[^}]*\}/s)?.[0] || '';

    // Design shows headers in title case, not uppercase
    expect(thBlock).not.toMatch(/text-transform:\s*uppercase/);
  });
});
