import { type Page, type Locator } from '@playwright/test';

export abstract class BasePage {
  constructor(protected readonly page: Page) {}

  abstract readonly path: string;

  async navigate(): Promise<void> {
    await this.page.goto(this.path);
    await this.waitForReady();
  }

  async waitForReady(): Promise<void> {
    await this.page.waitForLoadState('networkidle');
  }

  get toastMessage(): Locator {
    return this.page.locator('[data-testid="toast-message"]');
  }

  async getToastText(): Promise<string> {
    await this.toastMessage.waitFor({ state: 'visible' });
    return await this.toastMessage.innerText();
  }
}
