const {test,expect}=require('@playwright/test');
for(const path of [
  '/customer/profile','/notifications',
  '/customer/providers/AUTOMATION-NOT-A-REAL-PROVIDER/contact',
  '/customer/providers/AUTOMATION-NOT-A-REAL-PROVIDER/directions',
  '/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/tracking'
]){
  test(`anonymous request to ${path} redirects without exposing PII`,async ({request})=>{
    const r=await request.get(path,{maxRedirects:0,headers:{Accept:'application/json'}});
    expect(r.status()).toBe(302);
    const location=r.headers()['location'];
    expect(location).toBeTruthy();
    expect(location==='/' || location==='/login'||location.endsWith('/')).toBe(true);
    const body=await r.text();
    expect(body).not.toMatch(/phone|whatsapp|latitude|longitude|password|otp|Traceback/i);
  });
}
