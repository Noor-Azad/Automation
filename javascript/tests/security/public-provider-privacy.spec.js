const {test,expect}=require('@playwright/test');
const forbidden=['phone','whatsapp','email','user_id','password','password_hash','latitude','longitude','registered_address','pincode'];
function assertPrivateFieldsAbsent(provider){for(const field of forbidden)expect(provider,{message:`Private field exposed: ${field}`}).not.toHaveProperty(field);}
test('public provider search does not expose private fields',async({request})=>{
  const r=await request.get('/api/search/providers',{params:{limit:50,offset:0,sort:'rating-high'}});
  expect(r.status()).toBe(200);
  for(const provider of (await r.json()).data.providers){assertPrivateFieldsAbsent(provider);expect(String(provider.id).trim()).not.toBe('');}
});
test('public provider profile has no private contact or location fields',async({request})=>{
  const search=await request.get('/api/search/providers',{params:{limit:1,offset:0,sort:'rating-high'}});
  expect(search.status()).toBe(200);
  const providers=(await search.json()).data.providers;
  if(!providers.length)return;
  const r=await request.get('/api/providers/'+encodeURIComponent(providers[0].id));
  expect(r.status()).toBe(200);
  const profile=await r.json();
  assertPrivateFieldsAbsent(profile);
  expect(Array.isArray(profile.services)).toBe(true);
  expect(Array.isArray(profile.recent_reviews)).toBe(true);
});
test('unknown public provider profile returns clean 404',async({request})=>{
  const r=await request.get('/api/providers/AUTOMATION-NOT-A-REAL-PROVIDER');
  expect(r.status()).toBe(404);
  expect((await r.json()).error).toBe('Provider not found');
});
