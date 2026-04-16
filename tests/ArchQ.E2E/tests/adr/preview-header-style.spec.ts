import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-062: Preview header padding and font-size match design', () => {

  test('preview-header should use 0.5rem 0.75rem padding and 0.6875rem font-size', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.preview-header\s*\{[^}]*\}/s)?.[0] || '';

    // Padding should be 0.5rem 0.75rem (8px 12px), not 0.5rem 1rem (8px 16px)
    expect(block).toContain('padding: 0.5rem 0.75rem');
    expect(block).not.toMatch(/padding:\s*0\.5rem\s+1rem/);

    // Font-size should be 0.6875rem (11px), not 0.75rem (12px)
    expect(block).toContain('font-size: 0.6875rem');
    expect(block).not.toMatch(/font-size:\s*0\.75rem/);
  });
});
