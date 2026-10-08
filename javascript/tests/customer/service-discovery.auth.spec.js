const {test,expect}=require('../../fixtures/auth');
const {CustomerDashboardPage}=require('../../pages/CustomerDashboardPage');
test.describe.configure({mode:'serial'});
test('customer can load and filter service catalogue',async({authenticatedPage:page})=>{
 const d=new CustomerDashboardPage(page);
 await d.openServicesFromHome();
 await expect(d.servicesScreen).toHaveClass(/active/);
 await d.waitForServiceCatalogue();
 expect(await d.serviceCards.count()).toBeGreaterThan(0);
 const firstName=(await d.serviceCards.first().locator('.service-name').innerText()).trim();
 expect(firstName).not.toBe('');
 await d.filterServices(firstName.slice(0,3));
 await expect(d.serviceCards.filter({hasText:firstName}).first()).toBeVisible();
});
test('selecting a service triggers provider search successfully',async({authenticatedPage:page})=>{
 const d=new CustomerDashboardPage(page);
 await d.openServicesFromHome();
 await d.waitForServiceCatalogue();
 const responsePromise=page.waitForResponse(r=>r.url().includes('/api/search/providers?')&&r.request().method()==='GET');
 await d.selectFirstService();
 expect((await responsePromise).status()).toBe(200);
 await expect(d.serviceResultsScreen).toHaveClass(/active/);
 await expect(d.providerResults.locator('.error-state')).toHaveCount(0);
 await expect(d.resultsSubtitle).toBeVisible();
});
