const {test,expect}=require('../../fixtures/auth');
test.describe.configure({mode:'serial'});
test('customer dashboard has no CSP execution errors',async({authenticatedPage:page})=>{
 const errors=[];page.on('console',m=>{if(m.type()==='error'&&m.text().includes('Content Security Policy'))errors.push(m.text());});
 await page.goto('/customer/dashboard');await page.waitForLoadState('domcontentloaded');
 expect(errors).toEqual([]);
});
