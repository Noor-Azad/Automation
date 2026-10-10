const {test,expect} = require('@playwright/test');
test('public HTML has expected security headers',async ({request})=>{
  const r=await request.get('/');
  expect(r.status()).toBe(200);
  const h=r.headers();
  expect(h['x-content-type-options']).toBe('nosniff');
  expect(h['x-frame-options']).toBe('DENY');
  expect(h['referrer-policy']).toBe('strict-origin-when-cross-origin');
  expect(h['content-security-policy']).toContain("frame-ancestors 'none'");
  expect(h['content-security-policy']).toContain("object-src 'none'");
  expect(h['content-security-policy']).not.toContain("'unsafe-eval'");
});
for(const path of ['/customer/dashboard','/provider/dashboard','/admin/dashboard']){
  test(`anonymous browser redirects away from ${path}`,async ({page})=>{
    await page.goto(path);
    expect(new URL(page.url()).pathname).toMatch(/^\/$|^\/login$/);
  });
}
for(const [latitude,longitude] of [['0 OR 1=1','77.1025'],['25.0',"77.0' OR '1'='1"],['NaN','77.1025'],['91','77.1025']]){
  test(`service search rejects invalid coordinates ${latitude} / ${longitude}`,async ({request})=>{
    const r=await request.get('/api/services',{params:{latitude,longitude}});
    expect(r.status()).toBe(400);
    const body=await r.text();
    expect(body).not.toMatch(/Traceback|sqlalchemy|psycopg/i);
  });
}
test('anonymous session API does not expose credentials',async ({request})=>{
  const r=await request.get('/api/session');
  expect(r.status()).toBe(401);
  expect(await r.text()).not.toMatch(/password|secret/i);
});
