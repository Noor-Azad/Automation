const {test,expect}=require('@playwright/test');
const {WelcomePage}=require('../../pages/WelcomePage');
const {OtpPage}=require('../../pages/OtpPage');
test('customer login rotates anonymous session cookie',async({page})=>{
 test.skip(process.env.TEDILE_RUN_OTP_RATE_LIMITED!=='true','Explicit OTP regression opt-in required');
 const phone=process.env.TEDILE_E2E_CUSTOMER_PHONE,otp=process.env.TEDILE_E2E_CUSTOMER_OTP;
 expect(phone).toBeTruthy();expect(otp).toBeTruthy();
 await page.context().clearCookies();
 const welcome=new WelcomePage(page);await welcome.open();
 const before=(await page.context().cookies()).find(x=>x.name==='session');
 expect(before).toBeDefined();
 await welcome.requestCustomerOtp(phone);await page.waitForURL('**/otp');
 await new OtpPage(page).verify(otp);await page.waitForURL('**/customer/dashboard');
 const after=(await page.context().cookies()).find(x=>x.name==='session');
 expect(after).toBeDefined();expect(after.value).not.toBe(before.value);
 const r=await page.request.get('/api/session');
 expect(r.status()).toBe(200);expect((await r.json()).authenticated).toBe(true);
});
