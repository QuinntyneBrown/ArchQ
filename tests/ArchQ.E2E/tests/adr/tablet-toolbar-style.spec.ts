import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-047: Tablet toolbar has proper styling', () => {

  test('Tablet toolbar CSS should have background-color and width', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/app.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Find the .app-toolbar block inside the media query
    const mediaBlock = scss.match(/@media\s*\(max-width:\s*1024px\)\s*\{[\s\S]*?\n\}/)?.[0] || '';

    // Should have background-color and width in the toolbar rule
    expect(mediaBlock).toMatch(/\.app-toolbar[\s\S]*?background-color:\s*#1A1D27/i);
    expect(mediaBlock).toMatch(/\.app-toolbar[\s\S]*?width:\s*100%/);
  });
});
