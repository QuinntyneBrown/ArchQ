import { type Page, type Locator } from '@playwright/test';
import { BasePage } from './base.page';

export class TenantDetailPage extends BasePage {
  readonly path = '/tenants';

  readonly tenantName: Locator;
  readonly tenantSlug: Locator;
  readonly tenantStatus: Locator;
  readonly editNameButton: Locator;
  readonly displayNameInput: Locator;
  readonly saveNameButton: Locator;
  readonly cancelEditButton: Locator;

  constructor(page: Page) {
    super(page);
    this.tenantName = page.locator('[data-testid="tenant-name"]');
    this.tenantSlug = page.locator('[data-testid="tenant-slug"]');
    this.tenantStatus = page.locator('[data-testid="tenant-status"]');
    this.editNameButton = page.locator('[data-testid="edit-name-button"]');
    this.displayNameInput = page.locator('[data-testid="tenant-display-name-input"]');
    this.saveNameButton = page.locator('[data-testid="save-name-button"]');
    this.cancelEditButton = page.locator('[data-testid="cancel-edit-button"]');
  }

  async navigateToTenant(id: string): Promise<void> {
    await this.page.goto(`${this.path}/${id}`);
    await this.waitForReady();
  }

  async editDisplayName(newName: string): Promise<void> {
    await this.editNameButton.click();
    await this.displayNameInput.clear();
    await this.displayNameInput.fill(newName);
    await this.saveNameButton.click();
  }
}
