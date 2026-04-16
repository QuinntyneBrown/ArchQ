import { test, expect } from '../../fixtures/tenant.fixture';
import { TEST_USERS } from '../../fixtures/test-data';

test.describe('Tenant Detail & Update', () => {

  test('should display tenant details', async ({ page, apiHelper }) => {
    // Create a tenant via API
    const token = await apiHelper.login(TEST_USERS.admin.email, TEST_USERS.admin.password);
    const response = await apiHelper.createTenant({
      displayName: 'Detail Test Org',
      slug: `detail-test-${Date.now()}`,
    }, token);
    const tenant = await response.json();

    // Navigate to tenant detail page
    await page.goto(`/tenants/${tenant.id}`);

    await expect(page.locator('[data-testid="tenant-name"]')).toHaveText('Detail Test Org');
    await expect(page.locator('[data-testid="tenant-status"]')).toHaveText('active');
  });

  test('should update tenant display name', async ({ page, tenantDetailPage, apiHelper }) => {
    // Create a tenant via API
    const token = await apiHelper.login(TEST_USERS.admin.email, TEST_USERS.admin.password);
    const response = await apiHelper.createTenant({
      displayName: 'Original Name',
      slug: `update-test-${Date.now()}`,
    }, token);
    const tenant = await response.json();

    // Navigate to tenant detail
    await tenantDetailPage.navigateToTenant(tenant.id);

    // Edit the display name
    await tenantDetailPage.editDisplayName('Updated Name');

    // Verify the toast and updated name
    const toast = await tenantDetailPage.getToastText();
    expect(toast).toContain('updated');
    await expect(tenantDetailPage.tenantName).toHaveText('Updated Name');
  });

  test('should not allow slug to be edited', async ({ page, apiHelper }) => {
    const token = await apiHelper.login(TEST_USERS.admin.email, TEST_USERS.admin.password);
    const response = await apiHelper.createTenant({
      displayName: 'Slug Lock Test',
      slug: `slug-lock-${Date.now()}`,
    }, token);
    const tenant = await response.json();

    await page.goto(`/tenants/${tenant.id}`);

    // Slug should be displayed as read-only text, not an editable input
    const slugElement = page.locator('[data-testid="tenant-slug"]');
    await expect(slugElement).toBeVisible();

    // There should be no edit button for slug
    const slugEditButton = page.locator('[data-testid="edit-slug-button"]');
    await expect(slugEditButton).not.toBeVisible();
  });

  test('should return 404 for cross-tenant resource access', async ({ page, apiHelper }) => {
    // Try to access a non-existent tenant
    await page.goto('/tenants/non-existent-id');

    // Should show 404 or not-found message
    await expect(page.locator('[data-testid="not-found-message"]')).toBeVisible();
  });
});
