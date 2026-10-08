const {test,expect}=require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
test('authenticated customer session returns role without private fields',async({authenticatedPage:page})=>{
 const r=await page.request.get('/api/session');
 expect(r.status()).toBe(200);
 const data=await r.json();
 expect(data.authenticated).toBe(true);
 expect(data.user.role).toBe('customer');
 expect(JSON.stringify(data)).not.toMatch(/password|phone|email|otp/i);
});
