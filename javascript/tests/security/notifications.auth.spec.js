const {test,expect}=require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
test('authenticated notifications index has scoped contract',async({authenticatedPage:page})=>{
 const r=await page.request.get('/notifications');
 expect(r.status()).toBe(200);
 const body=await r.json();
 expect(Array.isArray(body.data)).toBe(true);
 expect(body.unread).toBeGreaterThanOrEqual(0);
 expect(JSON.stringify(body)).not.toMatch(/password|otp|password_hash/i);
});
for(const [validCsrf,status] of [[true,404],[false,400]]){
 test(`unknown notification mark read with csrf=${validCsrf} returns ${status}`,async({authenticatedPage:page})=>{
  const result=await page.evaluate(async(validCsrf)=>{
    const csrf=document.querySelector('meta[name="csrf-token"]')?.content||'';
    const headers={Accept:'application/json'};
    if(validCsrf)headers['X-CSRFToken']=csrf;
    const r=await fetch('/notifications/2147483647/read',{method:'POST',credentials:'same-origin',headers});
    return {status:r.status,body:await r.text()};
  },validCsrf);
  expect(result.status).toBe(status);
  if(validCsrf)expect(result.body).toContain('Notification not found');
  else expect(result.body).not.toContain('Notification not found');
  expect(result.body).not.toMatch(/Traceback/i);
 });
}
for(const path of ['/notifications/2147483647/read','/notifications/clear']){
 test(`GET ${path} is forbidden by method`,async({authenticatedPage:page})=>{
  const r=await page.request.get(path,{maxRedirects:0});
  expect(r.status()).toBe(405);
 });
}
