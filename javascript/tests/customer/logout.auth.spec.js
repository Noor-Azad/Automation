const { test, expect } = require('../../fixtures/auth');
const { CustomerDashboardPage } = require('../../pages/CustomerDashboardPage');

test.describe.configure({ mode: 'serial' });

test('customer logout returns to welcome and ends session', async ({ authenticatedPage: page }) => {
  const before = await page.request.get('/api/session');
  expect(before.status()).toBe(200);
  expect((await before.json()).authenticated).toBe(true);

  const previousCookie = (await page.context().cookies()).find(c => c.name === 'session');
  expect(previousCookie).toBeDefined();

  const dashboard = new CustomerDashboardPage(page);
  await dashboard.openAccount();
  await dashboard.logout();

  await expect(page.locator("[data-auth-form='customer-login']")).toBeVisible();
  const after = await page.request.get('/api/session');
  expect(after.status()).toBe(401);

  const newCookie = (await page.context().cookies()).find(c => c.name === 'session');
  if (newCookie) expect(newCookie.value).not.toBe(previousCookie.value);
});
