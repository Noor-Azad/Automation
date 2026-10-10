class WelcomePage {
  constructor(page) {
    this.page = page;
    this.title = page.locator('#welcome-title');
    this.customerTab = page.locator('#persona-customer');
    this.providerTab = page.locator('#persona-provider');
    this.customerForm = page.locator("[data-auth-form='customer-login']");
    this.providerForm = page.locator("[data-auth-form='provider-login']");
    this.customerPhone = page.locator('#customer-login-phone');
    this.customerSendCode = this.customerForm.getByRole('button', { name: 'Send code' });
    this.error = page.getByRole('alert');
    this.termsLink = page.getByRole('link', { name: 'Terms of Service' });
    this.privacyLink = page.getByRole('link', { name: 'Privacy Policy' });
  }

  async open() {
    await this.page.goto('/');
    await this.page.waitForLoadState('domcontentloaded');
  }

  async selectCustomer() { await this.customerTab.click(); }
  async selectProvider() { await this.providerTab.click(); }

  async requestCustomerOtp(phone) {
    await this.selectCustomer();
    // The visible input expects 10 national digits; test credentials may be +91-prefixed.
    const nationalNumber = /^\+91\d{10}$/.test(phone) ? phone.slice(3) : phone;
    await this.customerPhone.fill(nationalNumber);
    await this.customerSendCode.click();
  }
}
module.exports = { WelcomePage };
