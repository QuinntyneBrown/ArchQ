import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-079: Table header should use Geist Mono font-family', () => {

  test('adr-table th should include Geist Mono font-family', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.adr-table\s+th\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toMatch(/font-family:.*Geist Mono/);
  });
});
