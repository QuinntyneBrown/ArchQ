import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-068: Author avatar matches design Avatar component', () => {

  test('author-avatar should use #4F46E5 bg, #FFFFFF text, 2rem size, 0.75rem font', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const block = scss.match(/\.author-avatar\s*\{[^}]*\}/s)?.[0] || '';

    // Background should be indigo #4F46E5, not gray #374151
    expect(block).toMatch(/#4F46E5/i);
    expect(block).not.toMatch(/#374151/i);

    // Text color should be white #FFFFFF, not #d1d5db
    expect(block).toMatch(/color:\s*#FFFFFF/i);

    // Size should be 2rem (32px), not 1.75rem (28px)
    expect(block).toContain('width: 2rem');
    expect(block).toContain('height: 2rem');

    // Font-size should be 0.75rem (12px), not 0.625rem (10px)
    expect(block).toContain('font-size: 0.75rem');
  });
});
