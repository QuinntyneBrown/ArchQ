import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-072: Tablet table header font-size should be 0.6875rem (11px)', () => {

  test('tablet media query th should include font-size 0.6875rem', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Find the tablet media query (768-1024)
    const tabletStart = scss.indexOf('@media (min-width: 768px) and (max-width: 1024px)');
    expect(tabletStart).toBeGreaterThan(-1);

    const tabletSection = scss.substring(tabletStart);
    const nextMedia = tabletSection.indexOf('@media', 1);
    const tabletBlock = nextMedia > 0 ? tabletSection.substring(0, nextMedia) : tabletSection;

    // th should have font-size: 0.6875rem
    expect(tabletBlock).toMatch(/\.adr-table\s+th\s*\{[^}]*font-size:\s*0\.6875rem/s);
  });
});
