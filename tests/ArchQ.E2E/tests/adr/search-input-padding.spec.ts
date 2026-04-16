import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-069: Search input vertical padding should be 0.625rem (10px)', () => {

  test('search-input should use 0.625rem vertical padding', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.search-input\s*\{[^}]*\}/s)?.[0] || '';

    // Padding should use 0.625rem for top and bottom (10px)
    expect(block).toContain('padding: 0.625rem 0.75rem 0.625rem 2.25rem');
  });
});
