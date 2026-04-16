import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-060: Search result title consistent with card title', () => {

  test('search-result-title should use 0.875rem (14px) like card-title', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.search-result-title\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toMatch(/font-size:\s*0\.875rem/);
    expect(block).not.toMatch(/font-size:\s*0\.9375rem/);
  });
});
