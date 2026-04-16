import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-071: Tablet table padding matches design', () => {

  test('tablet media query should use correct th/td padding', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Find the tablet media query (768-1024)
    const tabletStart = scss.indexOf('@media (min-width: 768px) and (max-width: 1024px)');
    expect(tabletStart).toBeGreaterThan(-1);

    const tabletSection = scss.substring(tabletStart);
    const nextMedia = tabletSection.indexOf('@media', 1);
    const tabletBlock = nextMedia > 0 ? tabletSection.substring(0, nextMedia) : tabletSection;

    // Should NOT have the old combined 0.5rem horizontal padding
    expect(tabletBlock).not.toMatch(/padding:\s*0\.75rem\s+0\.5rem/);

    // th should have 0.625rem 1rem (10px 16px)
    expect(tabletBlock).toMatch(/\.adr-table\s+th\s*\{[^}]*padding:\s*0\.625rem\s+1rem/s);

    // td should have 0.75rem 1rem (12px 16px)
    expect(tabletBlock).toMatch(/\.adr-table\s+td\s*\{[^}]*padding:\s*0\.75rem\s+1rem/s);
  });
});
