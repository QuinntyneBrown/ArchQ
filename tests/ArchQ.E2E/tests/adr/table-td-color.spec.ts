import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-051: Table td uses $text-primary', () => {

  test('Table td CSS should use #F0F1F5, not #D1D5DB', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.adr-table td\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toMatch(/#F0F1F5/i);
    expect(block).not.toMatch(/#d1d5db/i);
  });
});
