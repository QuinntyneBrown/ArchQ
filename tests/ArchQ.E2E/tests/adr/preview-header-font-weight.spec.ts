import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-081: Preview header font-weight should be normal', () => {

  test('preview-header should use font-weight normal, not 600', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.preview-header\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toMatch(/font-weight:\s*normal/);
    expect(block).not.toMatch(/font-weight:\s*600/);
  });
});
