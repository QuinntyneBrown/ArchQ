import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-048: Date cell uses $text-disabled', () => {

  test('Date cell CSS should use #5C5F6E, not #9CA3AF', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.date-cell\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toMatch(/#5C5F6E/i);
    expect(block).not.toMatch(/#9ca3af/i);
  });
});
