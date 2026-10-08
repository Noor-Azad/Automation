const {test,expect}=require('../../fixtures/auth');
const {ProviderProfilePage}=require('../../pages/ProviderProfilePage');
const {findBookableProvider}=require('../../utils/provider');
test.describe.configure({mode:'serial'});
async function openBooking(page){
 const id=await findBookableProvider(page.request);
 await page.goto('/providers/'+encodeURIComponent(id));
 const p=new ProviderProfilePage(page);await p.waitForLoaded();await p.openBookingPanel();return p;
}
for(const [name,fill,errorSelector,errorMessage] of [
 ['invalid name',async p=>p.customerName.fill('12345'),'nameError','Please enter a valid name using letters, spaces, and dots only.'],
 ['invalid service address',async p=>{await p.customerName.fill('Automation Customer');await p.addressLine1.fill('test');await p.addressLocality.fill('Malda');await p.addressPincode.fill('732101');},'addressError','Please enter a proper service address.'],
 ['invalid PIN code',async p=>{await p.customerName.fill('Automation Customer');await p.addressLine1.fill('Station Road');await p.addressLocality.fill('Malda');await p.addressPincode.fill('1234');},'addressError','Enter a valid 6-digit PIN code.']
]){
 test(`booking form rejects ${name} without POST`,async({authenticatedPage:page})=>{
  const p=await openBooking(page);let bookingPosts=0;
  page.on('request',r=>{if(r.method()==='POST'&&new URL(r.url()).pathname==='/customer/bookings')bookingPosts++;});
  await fill(p);await p.submitButton.click();await expect(p[errorSelector]).toHaveText(errorMessage);
  expect(bookingPosts).toBe(0);
 });
}
