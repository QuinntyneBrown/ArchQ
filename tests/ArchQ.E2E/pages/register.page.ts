import { type Page, type Locator } from '@playwright/test';
import { BasePage } from './base.page';

export class RegisterPage extends BasePage {
  readonly path = '/register';

  readonly fullNameInput: Locator;
  readonly emailInput: Locator;
  readonly passwordInput: Locator;
  readonly organizationNameInput: Locator;
  readonly createAccountButton: Locator;
  readonly signInLink: Locator;
  readonly successMessage: Locator;
  readonly errorMessage: Locator;
  readonly fullNameError: Locator;
  readonly emailError: Locator;
  readonly passwordError: Locator;
  readonly organizationError: Locator;
  readonly passwordStrength: Locator;

  constructor(page: Page) {
    super(page);
    this.fullNameInput = page.locator('[data-testid="full-name-input"]');
    this.emailInput = page.locator('[data-testid="email-input"]');
    this.passwordInput = page.locator('[data-testid="password-input"]');
    this.organizationNameInput = page.locator('[data-testid="organization-name-input"]');
    this.createAccountButton = page.locator('[data-testid="create-account-button"]');
    this.signInLink = page.locator('[data-testid="sign-in-link"]');
    this.successMessage = page.locator('[data-testid="success-message"]');
    this.errorMessage = page.locator('[data-testid="error-message"]');
    this.fullNameError = page.locator('[data-testid="full-name-error"]');
    this.emailError = page.locator('[data-testid="email-error"]');
    this.passwordError = page.locator('[data-testid="password-error"]');
    this.organizationError = page.locator('[data-testid="organization-error"]');
    this.passwordStrength = page.locator('[data-testid="password-strength"]');
  }

  async register(fullName: string, email: string, password: string, orgName: string): Promise<void> {
    await this.fullNameInput.fill(fullName);
    await this.emailInput.fill(email);
    await this.passwordInput.fill(password);
    await this.organizationNameInput.fill(orgName);
    await this.createAccountButton.click();
  }
}
