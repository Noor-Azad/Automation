const {test,expect}=require('@playwright/test');
for(const [path,method] of [
  ['/customer/bookings','POST'],['/provider/availability','POST'],
  ['/provider/working-hours','PUT'],['/api/admin/providers','POST']
]){
  test(`anonymous ${method} ${path} is redirected before mutation`,async({request})=>{
    const r=await request.fetch(path,{method,maxRedirects:0,headers:{Accept:'application/json','Content-Type':'application/json'},data:{
      provider_profile_code:'AUTOMATION-NOT-A-REAL-PROVIDER',availability:'available',name:'Automation Should Not Create'
    }});
    expect(r.status()).toBe(302);
    const location=r.headers().location;
    expect(location==='/'||location==='/login'||location?.endsWith('/')).toBe(true);
    expect(await r.text()).not.toMatch(/created|updated|saved|Traceback/i);
  });
}
