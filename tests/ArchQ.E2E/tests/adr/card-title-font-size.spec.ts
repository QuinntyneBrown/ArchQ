import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-052: Card title font-size matches design', () => {

  test('Card title CSS should use 0.875rem (14px), not 0.9375rem (15px)', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.card-title\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toMatch(/font-size:\s*0\.875rem/);
    expect(block).not.toMatch(/font-size:\s*0\.9375rem/);
  });
});
