import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-042: New ADR button text matches design', () => {

  test('New ADR button should say "New ADR" not "+ New ADR"', async () => {
    const htmlPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/dashboard/adr-list/adr-list.component.html');
    const html = fs.readFileSync(htmlPath, 'utf-8');

    // Design specifies "New ADR" button text
    // Current implementation has "+ New ADR"
    expect(html).toContain('>New ADR<');
    expect(html).not.toContain('>+ New ADR<');
  });
});
