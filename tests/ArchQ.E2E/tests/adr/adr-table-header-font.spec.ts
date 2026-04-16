import { test, expect } from '@playwright/test';

test.describe('BUG-080: ADR table header should use Geist Mono font', () => {
  test.setTimeout(120000);

  test('table header th should have Geist Mono font-family in stylesheet', async ({ browser }) => {
    const context = await browser.newContext();
    const page = await context.newPage();

    // Log in to access the ADR list page (table CSS is loaded regardless of data)
    await page.goto('http://localhost:4200/login', { waitUntil: 'networkidle' });
    await page.fill('[data-testid="email-input"]', 'test-1776344632458@example.com');
    await page.fill('[data-testid="password-input"]', 'S3cur3P@ss!');
    await page.click('[data-testid="sign-in-button"]');
    await page.waitForURL(/\/adrs/, { timeout: 30000 });
    await page.waitForTimeout(1000);

    // Check the CSS rule for .adr-table th font-family by creating a temp th element
    const fontFamily = await page.evaluate(() => {
      const table = document.createElement('table');
      table.className = 'adr-table';
      const thead = document.createElement('thead');
      const tr = document.createElement('tr');
      const th = document.createElement('th');
      th.textContent = 'Test';
      tr.appendChild(th);
      thead.appendChild(tr);
      table.appendChild(thead);
      // Append inside the component host so scoped styles apply
      const container = document.querySelector('.adr-list-card') || document.body;
      container.appendChild(table);
      const computed = getComputedStyle(th).fontFamily;
      table.remove();
      return computed;
    });

    // Design spec: table headers use Geist Mono
    expect(
      fontFamily.toLowerCase(),
      'Table header font-family should include Geist Mono'
    ).toContain('geist mono');

    await context.close();
  });
});
