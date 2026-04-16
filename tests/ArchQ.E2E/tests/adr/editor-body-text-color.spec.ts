import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as path from 'path';

test.describe('BUG-058: Editor body text uses $text-primary', () => {

  test('Preview content and markdown editor should use #F0F1F5', async () => {
    const scssPath = path.resolve(__dirname, '../../../../src/ArchQ.Web/src/app/features/adr/adr-editor/adr-editor.component.scss');
    const scss = fs.readFileSync(scssPath, 'utf-8');

    const previewBlock = scss.match(/\.preview-content\s*\{[^}]*\}/s)?.[0] || '';
    const editorBlock = scss.match(/\.markdown-editor\s*\{[^}]*\}/s)?.[0] || '';

    expect(previewBlock).toMatch(/#F0F1F5/i);
    expect(previewBlock).not.toMatch(/#d1d5db/i);
    expect(editorBlock).toMatch(/#F0F1F5/i);
    expect(editorBlock).not.toMatch(/#d1d5db/i);
  });
});
