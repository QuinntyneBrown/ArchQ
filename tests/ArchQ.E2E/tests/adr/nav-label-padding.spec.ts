import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-067: Nav section label should have left padding 0.75rem', () => {

  test('nav-section-label CSS should include padding-left 0.75rem', () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/app.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const start = scss.indexOf('.nav-section-label {');
    const end = scss.indexOf('}', start);
    const block = scss.substring(start, end + 1);

    expect(block).toMatch(/padding-left:\s*0\.75rem/);
  });
});
