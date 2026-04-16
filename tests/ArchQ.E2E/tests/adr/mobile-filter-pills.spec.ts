import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-027: Mobile filter pills match design system', () => {

  test('Inactive pill CSS should use $bg-elevated #242736, not transparent', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Extract .pill background value
    const pillBlock = scss.match(/\.pill\s*\{[^}]*\}/s)?.[0] || '';

    // Should contain background-color: #242736 (or background: #242736)
    // Currently has background: transparent — which is wrong
    expect(pillBlock).toMatch(/#242736/i);
    expect(pillBlock).not.toMatch(/transparent/);
  });

  test('Inactive pill CSS should use $border-default #2E3142, not #3A3F54', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Extract .pill border value
    const pillBlock = scss.match(/\.pill\s*\{[^}]*\}/s)?.[0] || '';

    // Should use $border-default #2E3142
    expect(pillBlock).toMatch(/#2E3142/i);
    expect(pillBlock).not.toMatch(/#3a3f54/i);
  });
});
