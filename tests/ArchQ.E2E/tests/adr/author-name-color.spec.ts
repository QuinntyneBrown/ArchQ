import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-046: Author name uses $text-secondary', () => {

  test('Author name CSS should use #9CA3AF, not #D1D5DB', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.author-name\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toMatch(/#9CA3AF/i);
    expect(block).not.toMatch(/#d1d5db/i);
  });
});
