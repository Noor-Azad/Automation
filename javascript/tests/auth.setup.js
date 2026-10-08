const { test, expect } = require('@playwright/test');
const { mkdir } = require('node:fs/promises');
const path = require('node:path');
const { WelcomePage } = require('../pages/WelcomePage');
const { OtpPage } = require('../pages/OtpPage');

const authFile = path.join(__dirname, '..', 'playwright', '.auth', 'customer.json');

test('login dedicated review customer once', async ({ page }) => {
  const phone = process.env.TEDILE_E2E_CUSTOMER_PHONE;
  const code = process.env.TEDILE_E2E_CUSTOMER_OTP;
  expect(phone, 'Missing dedicated test phone').toBeTruthy();
  expect(code, 'Missing dedicated test OTP').toBeTruthy();

  const welcome = new WelcomePage(page);
  await welcome.open();
  await welcome.requestCustomerOtp(phone);
  await page.waitForURL('**/otp');
  await new OtpPage(page).verify(code);
  await page.waitForURL('**/customer/dashboard');
  await expect(page.locator('#screen-home')).toHaveClass(/active/);

  await mkdir(path.dirname(authFile), { recursive: true });
  await page.context().storageState({ path: authFile });
});
