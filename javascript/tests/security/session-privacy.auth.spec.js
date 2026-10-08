const {test,expect}=require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
test('session returns only minimal identity fields',async({authenticatedPage:page})=>{
  const response=await page.request.get('/api/session');
  expect(response.status()).toBe(200);
  const body=await response.json();
  expect(body.authenticated).toBe(true);
  for(const key of Object.keys(body.user))expect(['id','name','role','roles']).toContain(key);
  for(const field of ['id','name','role','roles'])expect(body.user).toHaveProperty(field);
  expect(JSON.stringify(body)).not.toMatch(/email|phone|password|password_hash|otp|secret/i);
});
for(const path of ['/customer/dashboard','/api/session','/customer/profile','/notifications',
'/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/tracking',
'/customer/providers/AUTOMATION-NOT-A-REAL-PROVIDER/contact']){
  test(`sensitive authenticated GET ${path} has no-store`,async({authenticatedPage:page})=>{
    const r=await page.request.get(path,{headers:{Accept:'application/json, text/html'},maxRedirects:0});
    expect([200,404]).toContain(r.status());
    expect(r.headers()['cache-control']).toContain('no-store');
  });
}
