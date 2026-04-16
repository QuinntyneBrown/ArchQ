import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-057: Sidebar and shared components use $text-primary', () => {

  test('Sidebar logo-text should use #F0F1F5, not #ffffff', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/app.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.logo-text\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toMatch(/#F0F1F5/i);
    expect(block).not.toMatch(/color:\s*#ffffff/i);
  });

  test('Org-switcher current org text should use #F0F1F5', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/shared/components/org-switcher/org-switcher.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // The main org-switcher display (not hover/active states)
    const switcherBlock = scss.match(/\.org-switcher\s*\{[^}]*\}/s)?.[0] || '';

    expect(switcherBlock).not.toMatch(/color:\s*#ffffff/i);
  });
});
