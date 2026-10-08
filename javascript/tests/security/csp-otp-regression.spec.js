const { test, expect } = require('@playwright/test');
const { WelcomePage } = require('../../pages/WelcomePage');

test('OTP page has no Content Security Policy console errors', async ({ page }) => {
  test.skip(
    process.env.TEDILE_RUN_OTP_RATE_LIMITED !== 'true' ||
    !process.env.TEDILE_E2E_CUSTOMER_PHONE,
    'Requires explicit opt-in for rate-limited OTP test'
  );
  const errors = [];
  page.on('console', message => {
    if (message.type() === 'error' && message.text().toLowerCase().includes('content security policy')) {
      errors.push(message.text());
    }
  });
  const welcome = new WelcomePage(page);
  await welcome.open();
  await welcome.requestCustomerOtp(process.env.TEDILE_E2E_CUSTOMER_PHONE);
  await page.waitForURL('**/otp');
  await page.waitForLoadState('domcontentloaded');
  expect(errors).toEqual([]);
});
