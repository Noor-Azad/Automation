const {test,expect}=require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
test('profile update cannot elevate account privileges',async({authenticatedPage:page})=>{
 const beforeRes=await page.request.get('/api/session');expect(beforeRes.status()).toBe(200);
 const before=(await beforeRes.json()).user;
 const result=await page.evaluate(async()=>{
  const csrf=document.querySelector('meta[name="csrf-token"]')?.content||'';
  const r=await fetch('/customer/profile',{method:'POST',credentials:'same-origin',headers:{Accept:'application/json','Content-Type':'application/json','X-CSRFToken':csrf},body:JSON.stringify({
   role:'admin',roles:['admin'],is_admin:true,is_test_account:true,is_google_play_review_account:true,onboarding_completed:true
  })});return {status:r.status,body:await r.text()};
 });
 expect(result.status).toBe(200);
 const afterRes=await page.request.get('/api/session');expect(afterRes.status()).toBe(200);
 const after=(await afterRes.json()).user;
 expect(after.role).toBe(before.role);expect(after.roles).toEqual(before.roles);
 expect(JSON.stringify(after)).not.toMatch(/is_admin|is_test_account|is_google_play_review_account/i);
});
