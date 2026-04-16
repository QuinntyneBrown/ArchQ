import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-050: Card author uses correct color and size', () => {

  test('Card author CSS should use #5C5F6E and 0.75rem', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.card-author\s*\{[^}]*\}/s)?.[0] || '';

    // Design: color #5C5F6E, fontSize 12px (0.75rem)
    expect(block).toMatch(/#5C5F6E/i);
    expect(block).toMatch(/font-size:\s*0\.75rem/);
  });
});
