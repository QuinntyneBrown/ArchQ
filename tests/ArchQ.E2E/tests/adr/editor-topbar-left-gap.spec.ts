import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-075: Editor top bar left gap should be 0.5rem (8px)', () => {

  test('top-bar-left should use 0.5rem gap', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.top-bar-left\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toMatch(/gap:\s*0\.5rem/);
    expect(block).not.toMatch(/gap:\s*0\.75rem/);
  });
});
