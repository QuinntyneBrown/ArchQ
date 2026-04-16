import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-066: Nav item font-weight 500 inactive / 600 active', () => {

  test('nav-item should have font-weight 500 and nav-active font-weight 600', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/app.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Check that font-weight: 500 appears in the .nav-item context
    // and font-weight: 600 appears in the .nav-active context
    const navItemStart = scss.indexOf('.nav-item {');
    expect(navItemStart).toBeGreaterThan(-1);

    // Get from .nav-item to the next top-level class (starts with \n.)
    const afterNavItem = scss.substring(navItemStart);
    const nextTopLevel = afterNavItem.indexOf('\n.', 1);
    const navItemSection = nextTopLevel > 0 ? afterNavItem.substring(0, nextTopLevel) : afterNavItem;

    expect(navItemSection).toMatch(/font-weight:\s*500/);
    expect(navItemSection).toMatch(/nav-active[\s\S]*font-weight:\s*600/);
  });
});
