import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-080: Preview header should not have text-transform uppercase', () => {

  test('preview-header should not use text-transform or letter-spacing', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.preview-header\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).not.toMatch(/text-transform:\s*uppercase/);
    expect(block).not.toMatch(/letter-spacing/);
  });
});
