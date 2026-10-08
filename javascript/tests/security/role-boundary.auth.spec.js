const {test,expect}=require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
test('customer cannot open provider or admin dashboards',async({authenticatedPage:page})=>{
  for(const path of ['/provider/dashboard','/admin/dashboard']){
    const r=await page.request.get(path,{maxRedirects:0});
    expect(r.status()).toBe(403);
  }
});
test('customer cannot update provider operational location',async({authenticatedPage:page})=>{
  const result=await page.evaluate(async()=>{
    const csrf=document.querySelector('meta[name="csrf-token"]')?.content||'';
    const r=await fetch('/api/providers/me/location',{method:'POST',credentials:'same-origin',headers:{
      Accept:'application/json','Content-Type':'application/json','X-CSRFToken':csrf
    },body:JSON.stringify({latitude:25.01,longitude:88.14,accuracy_meters:10})});
    return {status:r.status,body:await r.text()};
  });
  expect(result.status).toBe(403);
  expect(result.body).not.toMatch(/updated|Traceback/i);
});
test('customer contact for unknown provider returns clean not found',async({authenticatedPage:page})=>{
  const r=await page.request.get('/customer/providers/AUTOMATION-NOT-A-REAL-PROVIDER/contact');
  expect(r.status()).toBe(404);
  const body=await r.text();
  expect(JSON.parse(body).error).toBe('Provider not found');
  expect(body).not.toMatch(/phone|whatsapp|Traceback/i);
});
for(const [path,expected] of [
  ['/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/cancel','Booking not found'],
  ['/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/quote','Booking not found']
]){
  test(`nonexistent booking mutation ${path} is not found`,async({authenticatedPage:page})=>{
    const result=await page.evaluate(async({path})=>{
      const csrf=document.querySelector('meta[name="csrf-token"]')?.content||'';
      const r=await fetch(path,{method:'POST',credentials:'same-origin',headers:{
        Accept:'application/json','X-CSRFToken':csrf,
        ...(path.endsWith('/quote')?{'Content-Type':'application/x-www-form-urlencoded'}:{})
      },...(path.endsWith('/quote')?{body:new URLSearchParams({action:'accept',expected_amount:'100.00'})}:{})});
      return {status:r.status,body:await r.text()};
    },{path});
    expect(result.status).toBe(404);
    expect(result.body).toContain(expected);
  });
}
