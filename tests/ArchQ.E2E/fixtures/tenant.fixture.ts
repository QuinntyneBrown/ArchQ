import { test as base } from '@playwright/test';
import { TenantCreatePage } from '../pages/tenant-create.page';
import { TenantDetailPage } from '../pages/tenant-detail.page';
import { LoginPage } from '../pages/login.page';
import { ApiHelper } from '../helpers/api.helper';
import { TEST_USERS } from './test-data';

type TenantFixtures = {
  tenantCreatePage: TenantCreatePage;
  tenantDetailPage: TenantDetailPage;
  apiHelper: ApiHelper;
  adminPage: TenantCreatePage;
};

export const test = base.extend<TenantFixtures>({
  tenantCreatePage: async ({ page }, use) => {
    await use(new TenantCreatePage(page));
  },

  tenantDetailPage: async ({ page }, use) => {
    await use(new TenantDetailPage(page));
  },

  apiHelper: async ({ request }, use) => {
    await use(new ApiHelper(request));
  },

  adminPage: async ({ page }, use) => {
    const loginPage = new LoginPage(page);
    await loginPage.navigate();
    await loginPage.login(TEST_USERS.admin.email, TEST_USERS.admin.password);
    await page.waitForURL('**/adrs');
    const tenantCreatePage = new TenantCreatePage(page);
    await use(tenantCreatePage);
  },
});

export { expect } from '@playwright/test';
