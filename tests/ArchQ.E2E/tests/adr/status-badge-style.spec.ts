import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-033: Status badges should not have borders per design', () => {

  test('Status badge CSS should not contain border declarations', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    // Extract each status badge block and verify no border
    const draftBlock = scss.match(/\.status-draft\s*\{[^}]*\}/s)?.[0] || '';
    const reviewBlock = scss.match(/\.status-in-review\s*\{[^}]*\}/s)?.[0] || '';
    const approvedBlock = scss.match(/\.status-approved\s*\{[^}]*\}/s)?.[0] || '';
    const rejectedBlock = scss.match(/\.status-rejected\s*\{[^}]*\}/s)?.[0] || '';

    // Design badges have NO border - only tinted background
    expect(draftBlock).not.toMatch(/border:/);
    expect(reviewBlock).not.toMatch(/border:/);
    expect(approvedBlock).not.toMatch(/border:/);
    expect(rejectedBlock).not.toMatch(/border:/);
  });

  test('Status badge padding should be 4px 12px (0.25rem 0.75rem)', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const badgeBlock = scss.match(/\.status-badge\s*\{[^}]*\}/s)?.[0] || '';

    // Design uses padding [4, 12] = 0.25rem 0.75rem
    // Current uses 0.125rem 0.5rem (2px 8px)
    expect(badgeBlock).toMatch(/padding:\s*0\.25rem\s+0\.75rem/);
  });
});
