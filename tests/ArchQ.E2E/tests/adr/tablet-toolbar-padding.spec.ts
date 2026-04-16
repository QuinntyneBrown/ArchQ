import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-073: Tablet toolbar horizontal padding should be 1.25rem (20px)', () => {

  test('app-toolbar should use 0.75rem 1.25rem padding', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/app.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Find .app-toolbar block within media query
    const toolbarStart = scss.indexOf('.app-toolbar {');
    expect(toolbarStart).toBeGreaterThan(-1);
    const toolbarEnd = scss.indexOf('}', toolbarStart);
    const block = scss.substring(toolbarStart, toolbarEnd + 1);

    expect(block).toMatch(/padding:\s*0\.75rem\s+1\.25rem/);
    expect(block).not.toMatch(/padding:\s*0\.75rem\s+1rem/);
  });
});
