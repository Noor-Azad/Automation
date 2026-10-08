const {test,expect}=require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
async function postInvalidBooking(page,overrides){
 const values={
  customer_display_name:'Automation Customer',address_line_1:'Station Road',address_locality:'Malda',
  address_pincode:'732101',provider_profile_code:'NOT-A-REAL-PROVIDER',service_slug:'electrician',...overrides
 };
 return page.evaluate(async values=>{
  const csrf=document.querySelector('meta[name="csrf-token"]')?.content||'';
  const r=await fetch('/customer/bookings',{method:'POST',credentials:'same-origin',headers:{
   Accept:'application/json','Content-Type':'application/x-www-form-urlencoded','X-CSRFToken':csrf
  },body:new URLSearchParams(values)});
  return {status:r.status,body:await r.text()};
 },values);
}
for(const [name,overrides,message] of [
 ['invalid name',{customer_display_name:'12345'},'valid name'],
 ['invalid address',{address_line_1:'test'},'service address'],
 ['incomplete coordinates',{customer_latitude:'25.0'},'provided together'],
 ['nonfinite coordinates',{customer_latitude:'NaN',customer_longitude:'88.0'},'Invalid customer location coordinates'],
 ['oversized notes',{customer_latitude:'25.0',customer_longitude:'88.0',notes:'x'.repeat(5001)},'Notes are too long']
]){
 test(`booking API rejects ${name}`,async({authenticatedPage:page})=>{
  const result=await postInvalidBooking(page,overrides);
  expect(result.status).toBe(400);expect(result.body.toLowerCase()).toContain(message.toLowerCase());
 });
}
