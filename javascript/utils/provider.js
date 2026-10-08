const {expect}=require('@playwright/test');
async function findBookableProvider(request){
 const response=await request.get('/api/search/providers',{params:{limit:50,offset:0,sort:'rating-high'}});
 expect(response.status()).toBe(200);
 const providers=(await response.json()).data.providers;
 const item=providers.find(p=>p.availability?.toLowerCase()!=='offline' && String(p.id||'').trim());
 expect(item,'No non-offline provider in test catalogue').toBeTruthy();
 return String(item.id);
}
module.exports={findBookableProvider};
