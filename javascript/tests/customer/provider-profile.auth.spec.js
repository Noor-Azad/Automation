const {test,expect}=require('../../fixtures/auth');
const {ProviderProfilePage}=require('../../pages/ProviderProfilePage');
const {findBookableProvider}=require('../../utils/provider');
test.describe.configure({mode:'serial'});
test('customer can open real provider profile',async({authenticatedPage:page})=>{
 const id=await findBookableProvider(page.request);
 await page.goto('/providers/'+encodeURIComponent(id));
 const p=new ProviderProfilePage(page);await p.waitForLoaded();
 await expect(p.heading).toBeVisible();await expect(p.location).toBeVisible();
 expect(await p.services.count()).toBeGreaterThan(0);
});
test('booking sheet exposes read-only verified phone without submitting',async({authenticatedPage:page})=>{
 const id=await findBookableProvider(page.request);
 await page.goto('/providers/'+encodeURIComponent(id));
 const p=new ProviderProfilePage(page);await p.waitForLoaded();await p.openBookingPanel();
 await expect(p.bookingForm).toBeVisible();
 expect(await p.phoneField.getAttribute('readonly')).not.toBeNull();
 await expect(p.scheduledAt).toBeVisible();
 await expect(p.submitButton).toBeVisible();
 expect(await p.serviceSelect.count()===0||await p.serviceSelect.isVisible()).toBe(true);
});
