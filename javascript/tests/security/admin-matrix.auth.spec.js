const {test,expect}=require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
const fake='AUTOMATION-NOT-A-REAL-PROVIDER';
for(const action of ['allow-location-update','link-account','block','unblock']){
 test('customer forbidden admin provider '+action,async({authenticatedPage:page})=>{
  const outcome=await page.evaluate(async({fake,action})=>{
   const csrf=document.querySelector('meta[name="csrf-token"]')?.content||'';
   const r=await fetch('/api/admin/providers/'+fake+'/'+action,{method:'POST',credentials:'same-origin',headers:{Accept:'application/json','Content-Type':'application/json','X-CSRFToken':csrf},body:'{}'});
   return {status:r.status,body:await r.text()};
  },{fake,action});
  expect(outcome.status).toBe(403);expect(outcome.body).not.toMatch(/Traceback|updated|linked|blocked/i);
 });
}
for(const path of ['/api/admin/providers/'+fake+'/location-status','/admin/providers/new','/admin/providers/'+fake,'/admin/providers/2147483647/documents/service_agreement/download']){
 test('customer forbidden admin resource '+path,async({authenticatedPage:page})=>{
  const r=await page.request.get(path,{maxRedirects:0});expect(r.status()).toBe(403);
  expect(await r.text()).not.toMatch(/Traceback|location_update_allowed|Provider management/i);
 });
}
test('customer cannot approve provider documents',async({authenticatedPage:page})=>{
 const outcome=await page.evaluate(async()=>{
  const csrf=document.querySelector('meta[name="csrf-token"]')?.content||'';
  const r=await fetch('/admin/providers/2147483647/documents/service_agreement/review',{method:'POST',credentials:'same-origin',headers:{Accept:'application/json','Content-Type':'application/x-www-form-urlencoded','X-CSRFToken':csrf},body:new URLSearchParams({review_status:'approved',review_notes:'automation authorization boundary'})});
  return {status:r.status,body:await r.text()};
 });
 expect(outcome.status).toBe(403);expect(outcome.body).not.toMatch(/approved|reviewed|Traceback/i);
});
