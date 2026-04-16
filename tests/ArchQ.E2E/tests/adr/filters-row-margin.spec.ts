import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-074: Filters row margin-bottom should be 1.5rem (24px)', () => {

  test('filters-row should use 1.5rem margin-bottom', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.filters-row\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toMatch(/margin-bottom:\s*1\.5rem/);
    expect(block).not.toMatch(/margin-bottom:\s*1\.25rem/);
  });
});
