const {test,expect}=require('@playwright/test');
for(const [name,keyword] of [['SQL-like',"' OR 1=1--"],['XSS-like','<script>alert(1)</script>']]){
  test(`provider search safely handles ${name} keyword`,async({request})=>{
    const r=await request.get('/api/search/providers',{params:{keyword,limit:10,offset:0}});
    const body=await r.text();
    if(r.status()===403){expect(body).not.toMatch(/Traceback|sqlalchemy/i);return;}
    expect(r.status()).toBe(200);
    const json=JSON.parse(body);
    expect(json.status).toBe(true);
    expect(Array.isArray(json.data.providers)).toBe(true);
    if(name==='XSS-like')expect(body.toLowerCase()).not.toContain('<script>');
  });
}
for(const query of ['limit=0','limit=51','limit=abc','offset=-1','offset=abc','radius=-1',
'min_price=100&max_price=10','latitude=91&longitude=88','latitude=25',
'latitude=NaN&longitude=88']){
  test(`provider search rejects invalid query ${query}`,async({request})=>{
    const r=await request.get('/api/search/providers?'+query);
    expect(r.status()).toBe(400);
    expect(await r.json()).toHaveProperty('error');
  });
}
