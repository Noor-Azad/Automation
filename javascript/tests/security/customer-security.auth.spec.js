const {test,expect}=require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
async function readProfile(page){const r=await page.request.get('/customer/profile',{headers:{Accept:'application/json'}});expect(r.status()).toBe(200);return r.json();}
test('profile GET omits secret identity fields',async({authenticatedPage:page})=>{
 const data=await readProfile(page);const body=JSON.stringify(data);
 expect(data).toHaveProperty('name');expect(body).not.toMatch(/phone|email|password|otp/i);
});
test('profile update without csrf cannot change name',async({authenticatedPage:page})=>{
 const before=(await readProfile(page)).name;
 const outcome=await page.evaluate(async()=>{
  const r=await fetch('/customer/profile',{method:'POST',credentials:'same-origin',headers:{'Content-Type':'application/json'},body:JSON.stringify({name:'Should Not Be Applied'})});
  return r.status;
 });
 expect(outcome).toBe(400);expect((await readProfile(page)).name).toBe(before);
});
test('phone cannot be changed through customer profile',async({authenticatedPage:page})=>{
 const result=await page.evaluate(async()=>{
  const csrf=document.querySelector('meta[name="csrf-token"]')?.content||'';
  const r=await fetch('/customer/profile',{method:'POST',credentials:'same-origin',headers:{'Content-Type':'application/json','X-CSRFToken':csrf},body:JSON.stringify({phone:'+919876543210'})});
  return {status:r.status,body:await r.text()};
 });
 expect(result.status).toBe(400);expect(result.body).toContain('Phone number cannot be changed here.');
});
test('directions denied without active booking',async({authenticatedPage:page})=>{
 const r=await page.request.get('/customer/providers/NOT-A-REAL-PROVIDER/directions');
 expect(r.status()).toBe(403);expect(await r.text()).toMatch(/active booking journey/i);
});
