import { type Page, type Locator } from '@playwright/test';
import { BasePage } from './base.page';

export class TenantCreatePage extends BasePage {
  readonly path = '/tenants/new';

  readonly displayNameInput: Locator;
  readonly slugInput: Locator;
  readonly slugPreview: Locator;
  readonly createButton: Locator;
  readonly cancelButton: Locator;
  readonly errorMessage: Locator;
  readonly slugError: Locator;
  readonly displayNameError: Locator;

  constructor(page: Page) {
    super(page);
    this.displayNameInput = page.locator('[data-testid="tenant-display-name-input"]');
    this.slugInput = page.locator('[data-testid="tenant-slug-input"]');
    this.slugPreview = page.locator('[data-testid="tenant-slug-preview"]');
    this.createButton = page.locator('[data-testid="create-tenant-button"]');
    this.cancelButton = page.locator('[data-testid="cancel-button"]');
    this.errorMessage = page.locator('[data-testid="error-message"]');
    this.slugError = page.locator('[data-testid="slug-error"]');
    this.displayNameError = page.locator('[data-testid="display-name-error"]');
  }

  async createTenant(displayName: string, slug: string): Promise<void> {
    await this.displayNameInput.fill(displayName);
    await this.slugInput.fill(slug);
    await this.createButton.click();
  }
}
