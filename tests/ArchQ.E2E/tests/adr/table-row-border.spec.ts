import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-044: Table row border uses $border-subtle', () => {

  test('Table td border should use #1F2231 ($border-subtle), not #2E3142', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const tdBlock = scss.match(/\.adr-table td\s*\{[^}]*\}/s)?.[0] || '';

    // Design uses $border-subtle #1F2231 for row separators
    expect(tdBlock).toMatch(/#1F2231/i);
    expect(tdBlock).not.toMatch(/border.*#2E3142/i);
  });
});
