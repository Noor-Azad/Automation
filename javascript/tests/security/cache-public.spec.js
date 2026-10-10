const {test,expect}=require('@playwright/test');
test('public root cannot be cached due to session dependence',async({request})=>{
 const r=await request.get('/');
 expect(r.status()).toBe(200);
 expect(r.headers()['cache-control']).toContain('no-store');
});
