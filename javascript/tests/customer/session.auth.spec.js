const {test,expect} = require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
test('authenticated session cookie is secure and host-scoped',async ({authenticatedPage:page,baseURL})=>{
  const cookie=(await page.context().cookies()).find(x=>x.name==='session');
  expect(cookie).toBeDefined();
  expect(cookie.secure).toBe(true);
  expect(cookie.httpOnly).toBe(true);
  expect(cookie.sameSite).toBe('Lax');
  expect(cookie.domain).toContain(new URL(baseURL).hostname);
  expect(cookie.path).toBe('/');
});
for(const path of ['/logout','/customer/bookings','/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/review','/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/quote']){
  test(`GET ${path} rejects method while authenticated`,async ({authenticatedPage:page})=>{
    const r=await page.request.get(path,{maxRedirects:0});
    expect(r.status()).toBe(405);
  });
}
