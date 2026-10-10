const {test,expect} = require('../../fixtures/auth');
const {CustomerDashboardPage} = require('../../pages/CustomerDashboardPage');
test.describe.configure({mode:'serial'});
test('customer secondary screens navigate back deterministically',async ({authenticatedPage:page})=>{
  const d = new CustomerDashboardPage(page);
  await d.openServicesFromHome();
  await expect(d.servicesScreen).toHaveClass(/active/);
  await d.backFromServices();
  await expect(d.homeScreen).toHaveClass(/active/);
  await d.openAccount();
  await expect(d.accountScreen).toHaveClass(/active/);
  await d.openPersonalInformation();
  await expect(d.profileEditScreen).toHaveClass(/active/);
  await d.backFromProfileEdit();
  await expect(d.accountScreen).toHaveClass(/active/);
});
test('customer dashboard is visible after authenticated login',async ({authenticatedPage:page})=>{
  const d = new CustomerDashboardPage(page);
  await expect(d.homeScreen).toHaveClass(/active/);
  await expect(d.greeting).toBeVisible();
});
