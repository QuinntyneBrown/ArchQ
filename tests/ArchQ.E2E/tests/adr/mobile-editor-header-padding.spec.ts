import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-077: Mobile editor header padding should be 0.75rem 1rem', () => {

  test('mobile media query should override top-bar padding to 0.75rem 1rem', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Find the mobile media query
    const mobileStart = scss.indexOf('@media (max-width: 768px)');
    expect(mobileStart).toBeGreaterThan(-1);
    const mobileSection = scss.substring(mobileStart);

    // top-bar inside mobile should have padding 0.75rem 1rem
    expect(mobileSection).toMatch(/\.top-bar\s*\{[^}]*padding:\s*0\.75rem\s+1rem/s);
  });
});
