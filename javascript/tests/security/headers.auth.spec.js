const {test,expect}=require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
for(const path of ['/customer/dashboard','/api/session','/customer/profile','/notifications']){
 test('authenticated headers '+path,async({authenticatedPage:page})=>{
  const response=await page.request.get(path);expect([200,404]).toContain(response.status());
  const h=response.headers();
  expect(h['x-content-type-options']).toBe('nosniff');
  expect(h['x-frame-options']).toBe('DENY');
  expect(h['referrer-policy']).toBe('strict-origin-when-cross-origin');
  for(const policy of ['camera=()','microphone=()','geolocation=(self)'])expect(h['permissions-policy']).toContain(policy);
  for(const directive of ["object-src 'none'","frame-ancestors 'none'","base-uri 'self'"])expect(h['content-security-policy']).toContain(directive);
  expect(h['content-security-policy']).not.toContain("'unsafe-eval'");
  expect(h['cache-control']).toContain('no-store');
 });
}
