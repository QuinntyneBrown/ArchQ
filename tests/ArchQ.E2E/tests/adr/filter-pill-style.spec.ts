import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-065: Filter pill padding and font-weight match design', () => {

  test('pill should use 0.25rem vertical padding and normal font-weight', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const pillBlock = scss.match(/\.pill\s*\{[^}]*\}/s)?.[0] || '';
    const activeBlock = scss.match(/\.pill-active\s*\{[^}]*\}/s)?.[0] || '';

    // Vertical padding should be 0.25rem (4px), not 0.375rem (6px)
    expect(pillBlock).toContain('padding: 0.25rem 0.75rem');
    expect(pillBlock).not.toMatch(/padding:\s*0\.375rem/);

    // Inactive pill font-weight should be normal, not 500
    expect(pillBlock).toMatch(/font-weight:\s*normal/);
    expect(pillBlock).not.toMatch(/font-weight:\s*500/);

    // Active pill should have font-weight 600
    expect(activeBlock).toMatch(/font-weight:\s*600/);
  });
});
