import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-066: Nav item font-weight 500 inactive / 600 active', () => {

  test('nav-item should have font-weight 500 and nav-active font-weight 600', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/app.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Match the .nav-item block (including nested rules)
    const navItemMatch = scss.match(/\.nav-item\s*\{[^}]*(?:\{[^}]*\}[^}]*)*\}/s)?.[0] || '';

    // Inactive nav item should have font-weight: 500
    expect(navItemMatch).toMatch(/font-weight:\s*500/);

    // Active nav item should have font-weight: 600
    expect(navItemMatch).toMatch(/font-weight:\s*600/);
  });
});
