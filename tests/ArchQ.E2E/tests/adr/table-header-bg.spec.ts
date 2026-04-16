import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-045: Table header has elevated background', () => {

  test('Table th CSS should have background-color #242736', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const thBlock = scss.match(/\.adr-table th\s*\{[^}]*\}/s)?.[0] || '';

    // Design shows table header with $bg-elevated background
    expect(thBlock).toMatch(/background-color:\s*#242736/i);
  });
});
