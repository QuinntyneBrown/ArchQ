import { type Page, type Locator } from '@playwright/test';
import { BasePage } from './base.page';

export class VerifyEmailPage extends BasePage {
  readonly path = '/verify-email';

  readonly statusMessage: Locator;
  readonly errorMessage: Locator;
  readonly signInLink: Locator;

  constructor(page: Page) {
    super(page);
    this.statusMessage = page.locator('[data-testid="verification-status"]');
    this.errorMessage = page.locator('[data-testid="verification-error"]');
    this.signInLink = page.locator('[data-testid="sign-in-link"]');
  }
}
