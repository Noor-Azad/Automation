const {test,expect}=require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
async function browserMutation(page,path,method,data){
 return page.evaluate(async({path,method,data})=>{
   const csrf=document.querySelector('meta[name="csrf-token"]')?.content||'';
   const r=await fetch(path,{method,credentials:'same-origin',headers:{
    Accept:'application/json','Content-Type':'application/json','X-CSRFToken':csrf
   },body:JSON.stringify(data)});
   return {status:r.status,body:await r.text()};
 },{path,method,data});
}
for(const [path,method] of [
 ['/provider/profile/registered-location','POST'],
 ['/provider/availability','POST'],
 ['/provider/working-hours','PUT'],
 ['/provider/bookings/AUTOMATION-NOT-A-REAL-BOOKING/quote','POST']
]){
 test(`customer cannot ${method} provider-only ${path}`,async({authenticatedPage:page})=>{
   const r=await browserMutation(page,path,method,{availability:'available',latitude:25,longitude:88,amount:100});
   expect(r.status).toBe(403);
   expect(r.body).not.toMatch(/Traceback|updated|saved|quoted/i);
 });
}
for(const [path,method,data,forbidden] of [
 ['/api/admin/providers','POST',{name:'Automation Should Not Create',city:'Kolkata'},'created'],
 ['/api/admin/providers/AUTOMATION-NOT-A-REAL-PROVIDER','PATCH',{bio:'Should not be applied'},'updated']
]){
 test(`customer cannot ${method} admin API ${path}`,async({authenticatedPage:page})=>{
  const r=await browserMutation(page,path,method,data);
  expect(r.status).toBe(403);
  expect(r.body).not.toMatch(new RegExp(forbidden+'|Traceback','i'));
 });
}
test('customer cannot read provider owner working hours',async({authenticatedPage:page})=>{
 const r=await page.request.get('/provider/working-hours',{maxRedirects:0});
 expect(r.status()).toBe(403);
 expect(await r.text()).not.toMatch(/days|configured|Traceback/i);
});
