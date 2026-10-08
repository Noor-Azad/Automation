const {test,expect}=require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
async function getName(page){
 const r=await page.request.get('/customer/profile',{headers:{Accept:'application/json'}});
 expect(r.status()).toBe(200);return (await r.json()).name||'';
}
async function invalidProfile(page,values){
 return page.evaluate(async values=>{
  const csrf=document.querySelector('meta[name="csrf-token"]')?.content||'';
  const r=await fetch('/customer/profile',{method:'POST',credentials:'same-origin',headers:{
   Accept:'application/json','Content-Type':'application/json','X-CSRFToken':csrf
  },body:JSON.stringify(values)});
  return {status:r.status,body:await r.text()};
 },values);
}
for(const name of ['12345','test customer','<script>alert(1)</script>']){
 test(`invalid profile name rejected without mutation: ${name}`,async({authenticatedPage:page})=>{
  const before=await getName(page);
  const r=await invalidProfile(page,{name});expect(r.status).toBe(400);
  expect(r.body).toMatch(/valid name/i);expect(r.body).not.toMatch(/Traceback/i);
  expect(await getName(page)).toBe(before);
 });
}
test('invalid service address rejected without mutation',async({authenticatedPage:page})=>{
 const before=await getName(page);
 const r=await invalidProfile(page,{name:'Google Play Review',service_address:'unknown'});
 expect(r.status).toBe(400);expect(r.body).toMatch(/valid service address/i);
 expect(await getName(page)).toBe(before);
});
