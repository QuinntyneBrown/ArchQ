import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-082: Preview header should use Geist Mono font-family', () => {

  test('preview-header should include Geist Mono font-family', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.preview-header\s*\{[^}]*\}/s)?.[0] || '';

    expect(block).toMatch(/font-family:.*Geist Mono/);
  });
});
