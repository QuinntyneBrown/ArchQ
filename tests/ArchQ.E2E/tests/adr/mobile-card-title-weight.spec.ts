import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-035: Mobile card title font-weight matches design', () => {

  test('Card title CSS should use font-weight 600, not 500', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const cardTitleBlock = scss.match(/\.card-title\s*\{[^}]*\}/s)?.[0] || '';

    // Design specifies fontWeight 600 for card titles
    // Current implementation uses 500
    expect(cardTitleBlock).toMatch(/font-weight:\s*600/);
    expect(cardTitleBlock).not.toMatch(/font-weight:\s*500/);
  });
});
