import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-070: Mobile editor body padding should be 1rem (16px)', () => {

  test('mobile media query should override markdown-editor padding to 1rem', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Find the mobile media query block
    const mobileMediaStart = scss.indexOf('@media (max-width: 768px)');
    expect(mobileMediaStart).toBeGreaterThan(-1);

    const mobileSection = scss.substring(mobileMediaStart);

    // Should contain a .markdown-editor override with padding: 1rem
    expect(mobileSection).toMatch(/\.markdown-editor\s*\{[^}]*padding:\s*1rem/s);
  });
});
