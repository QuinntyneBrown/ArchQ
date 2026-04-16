import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-073: Tablet toolbar horizontal padding should be 1.25rem (20px)', () => {

  test('app-toolbar should use 0.75rem 1.25rem padding', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/app.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Find the media query block that contains .app-toolbar
    const mediaStart = scss.indexOf('@media (max-width: 1024px)');
    expect(mediaStart).toBeGreaterThan(-1);
    const mediaSection = scss.substring(mediaStart);

    // Within the media query, find .app-toolbar
    const toolbarMatch = mediaSection.match(/\.app-toolbar\s*\{[^}]*\}/s)?.[0] || '';

    expect(toolbarMatch).toMatch(/padding:\s*0\.75rem\s+1\.25rem/);
  });
});
