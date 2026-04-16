import { test, expect } from '../../fixtures/tenant.fixture';
import { TEST_USERS, TEST_TENANTS } from '../../fixtures/test-data';

test.describe('Tenant Provisioning', () => {

  test('should create a new tenant with valid display name and slug', async ({ adminPage, page }) => {
    await page.goto('/tenants/new');

    await adminPage.displayNameInput.fill('New Organization');
    await adminPage.slugInput.fill('new-organization');
    await adminPage.createButton.click();

    // Should show success toast
    const toast = await adminPage.getToastText();
    expect(toast).toContain('created');

    // Should redirect to tenant detail or dashboard
    await expect(page).toHaveURL(/\/tenants\/.+/);
  });

  test('should show slug preview when typing display name', async ({ adminPage, page }) => {
    await page.goto('/tenants/new');

    await adminPage.displayNameInput.fill('My Cool Org');

    // Slug preview should show auto-generated slug
    await expect(adminPage.slugPreview).toBeVisible();
    await expect(adminPage.slugPreview).toContainText('my-cool-org');
  });

  test('should show 409 conflict error when slug already exists', async ({ adminPage, page, apiHelper }) => {
    // First create a tenant via API to set up the conflict
    const token = await apiHelper.login(TEST_USERS.admin.email, TEST_USERS.admin.password);
    await apiHelper.createTenant({
      displayName: 'Existing Org',
      slug: 'existing-org',
    }, token);

    // Now try to create another with the same slug via UI
    await page.goto('/tenants/new');
    await adminPage.displayNameInput.fill('Another Org');
    await adminPage.slugInput.fill('existing-org');
    await adminPage.createButton.click();

    await expect(adminPage.errorMessage).toBeVisible();
    await expect(adminPage.errorMessage).toContainText('already in use');
  });

  test('should validate slug format — reject invalid characters', async ({ adminPage, page }) => {
    await page.goto('/tenants/new');

    await adminPage.displayNameInput.fill('Test Org');
    await adminPage.slugInput.fill('INVALID_SLUG!');
    await adminPage.createButton.click();

    await expect(adminPage.slugError).toBeVisible();
    await expect(adminPage.slugError).toContainText('lowercase alphanumeric');
  });

  test('should validate slug length — minimum 3 characters', async ({ adminPage, page }) => {
    await page.goto('/tenants/new');

    await adminPage.displayNameInput.fill('AB');
    await adminPage.slugInput.fill('ab');
    await adminPage.createButton.click();

    await expect(adminPage.slugError).toBeVisible();
    await expect(adminPage.slugError).toContainText('3');
  });

  test('should reject reserved slugs', async ({ adminPage, page }) => {
    await page.goto('/tenants/new');

    await adminPage.displayNameInput.fill('Admin Org');
    await adminPage.slugInput.fill('admin');
    await adminPage.createButton.click();

    await expect(adminPage.slugError).toBeVisible();
    await expect(adminPage.slugError).toContainText('reserved');
  });

  test('should require display name', async ({ adminPage, page }) => {
    await page.goto('/tenants/new');

    await adminPage.slugInput.fill('valid-slug');
    await adminPage.createButton.click();

    await expect(adminPage.displayNameError).toBeVisible();
  });

  test('should show tenant details after creation', async ({ adminPage, page }) => {
    await page.goto('/tenants/new');

    const uniqueSlug = `test-org-${Date.now()}`;
    await adminPage.createTenant('Test Organization', uniqueSlug);

    // Wait for redirect to tenant detail
    await expect(page).toHaveURL(/\/tenants\/.+/);

    // Verify details are displayed
    await expect(page.locator('[data-testid="tenant-name"]')).toHaveText('Test Organization');
    await expect(page.locator('[data-testid="tenant-slug"]')).toHaveText(uniqueSlug);
    await expect(page.locator('[data-testid="tenant-status"]')).toHaveText('active');
  });
});
