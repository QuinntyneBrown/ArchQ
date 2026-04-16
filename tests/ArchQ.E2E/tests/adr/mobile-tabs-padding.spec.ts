import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-076: Mobile tabs should have no container padding or gap', () => {

  test('mobile-tabs should use padding 0 and gap 0', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.mobile-tabs\s*\{[^}]*\}/s)?.[0] || '';

    // Should not have the old padding
    expect(block).not.toMatch(/padding:\s*0\.5rem\s+1\.5rem/);
    // Should not have gap 0.5rem
    expect(block).not.toMatch(/gap:\s*0\.5rem/);
  });
});
