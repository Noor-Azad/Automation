const { test: base, expect } = require('@playwright/test');
const { WelcomePage } = require('../pages/WelcomePage');
const { OtpPage } = require('../pages/OtpPage');

// No production OTP bypass. Authenticated tests are opt-in and use a dedicated test account.
// Run serially to avoid OTP rate limiting and account/session collision.
const test = base.extend({
  authenticatedPage: async ({browser, baseURL}, use) => {
    const phone = process.env.TEDILE_E2E_CUSTOMER_PHONE;
    const otpCode = process.env.TEDILE_E2E_CUSTOMER_OTP;
    test.skip(!phone || !otpCode || process.env.TEDILE_RUN_AUTHENTICATED !== 'true',
      'Requires dedicated test credentials and TEDILE_RUN_AUTHENTICATED=true');
    const context = await browser.newContext({baseURL});
    try {
      const page = await context.newPage();
      const welcome = new WelcomePage(page);
      await welcome.open();
      await welcome.requestCustomerOtp(phone);
      await page.waitForURL('**/otp');
      const otp = new OtpPage(page);
      await otp.verify(otpCode);
      await page.waitForURL('**/customer/dashboard');
      await use(page);
    } finally {
      await context.close();
    }
  }
});
module.exports = {test, expect};
