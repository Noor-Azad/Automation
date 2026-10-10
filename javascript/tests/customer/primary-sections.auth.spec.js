const { test, expect } = require('../../fixtures/auth');
const { CustomerDashboardPage } = require('../../pages/CustomerDashboardPage');

test.describe.configure({ mode: 'serial' });

test('authenticated customer opens primary dashboard sections', async ({ authenticatedPage: page }) => {
  expect(new URL(page.url()).pathname).toBe('/customer/dashboard');
  const dashboard = new CustomerDashboardPage(page);
  await expect(dashboard.homeScreen).toHaveClass(/active/);
  await expect(dashboard.greeting).toBeVisible();

  await dashboard.openBookings();
  await expect(dashboard.bookingsScreen).toHaveClass(/active/);

  await dashboard.openAccount();
  await expect(dashboard.accountScreen).toHaveClass(/active/);

  await dashboard.openPersonalInformation();
  await expect(dashboard.profileEditScreen).toHaveClass(/active/);
  await expect(dashboard.profilePhone).toBeVisible();
  expect(await dashboard.profilePhone.getAttribute('readonly')).not.toBeNull();
});
