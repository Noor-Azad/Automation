class ProviderProfilePage {
 constructor(page) {
  this.page=page; this.root=page.locator('#provider-profile');
  this.heading=this.root.locator('.provider-hero-body h1');
  this.location=this.root.locator('.provider-location');
  this.services=this.root.locator('.provider-service-card');
  this.reviewsSection=this.root.locator('.provider-profile-section').filter({hasText:'Reviews'});
  this.requestButton=page.locator('#start-booking');
  this.bookingPanel=page.locator('#booking-panel');
  this.bookingForm=page.locator('#booking-form');
  this.phoneField=this.bookingForm.locator('.booking-phone-field input');
  this.customerName=this.bookingForm.locator("input[name='customer_display_name']");
  this.addressLine1=this.bookingForm.locator("input[name='address_line_1']");
  this.addressLocality=this.bookingForm.locator("input[name='address_locality']");
  this.addressPincode=this.bookingForm.locator("input[name='address_pincode']");
  this.nameError=this.bookingForm.locator('.booking-name-error');
  this.addressError=this.bookingForm.locator('.booking-address-error');
  this.serviceSelect=this.bookingForm.locator('#booking-service');
  this.scheduledAt=this.bookingForm.locator("input[name='scheduled_at']");
  this.submitButton=this.bookingForm.getByRole('button',{name:'Send booking request'});
 }
 async waitForLoaded(){await this.heading.waitFor({state:'visible'});}
 async openBookingPanel(){await this.requestButton.click();await this.bookingPanel.waitFor({state:'visible'});}
}
module.exports={ProviderProfilePage};
