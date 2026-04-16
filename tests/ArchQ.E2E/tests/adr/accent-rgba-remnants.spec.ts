import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-036: No old accent blue rgba remnants in SCSS', () => {

  test('ADR list SCSS should not contain rgba(37, 99, 235) (old #2563EB)', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // rgba(37, 99, 235) is the old Tailwind blue #2563EB
    // Should be replaced with rgba(0, 98, 255) for accent-primary #0062FF
    expect(scss).not.toMatch(/rgba\(37,\s*99,\s*235/);
  });

  test('ADR list SCSS should use rgba(0, 98, 255) for accent highlights', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Accent-primary #0062FF = rgba(0, 98, 255)
    expect(scss).toMatch(/rgba\(0,\s*98,\s*255/);
  });
});
