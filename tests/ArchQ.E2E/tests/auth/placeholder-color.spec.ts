import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-030: Placeholder color matches design system $text-disabled', () => {

  test('Login SCSS placeholder color should use #5C5F6E, not #6B7280', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/auth/login/login.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // All instances of #6b7280 should be replaced with #5C5F6E ($text-disabled)
    expect(scss).not.toMatch(/#6b7280/i);
    expect(scss).toMatch(/#5C5F6E/i);
  });

  test('ADR list SCSS muted text color should use #5C5F6E, not #6B7280', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    expect(scss).not.toMatch(/#6b7280/i);
    expect(scss).toMatch(/#5C5F6E/i);
  });
});
