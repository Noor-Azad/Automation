const { test, expect } = require('@playwright/test');
const { WelcomePage } = require('../../pages/WelcomePage');
const { OtpPage } = require('../../pages/OtpPage');

test.describe.configure({ mode: 'serial' });
test.beforeEach(() => {
  test.skip(
    process.env.TEDILE_RUN_OTP_RATE_LIMITED !== 'true' ||
    !process.env.TEDILE_E2E_CUSTOMER_PHONE ||
    !process.env.TEDILE_E2E_CUSTOMER_OTP,
    'OTP regression requires explicit opt-in and dedicated test credentials'
  );
});

async function requestCode(page) {
  const welcome = new WelcomePage(page);
  await welcome.open();
  await welcome.requestCustomerOtp(process.env.TEDILE_E2E_CUSTOMER_PHONE);
  await page.waitForURL('**/otp');
  return { welcome, otpPage: new OtpPage(page) };
}

test('review customer cannot verify incorrect OTP', async ({ page }) => {
  const { otpPage } = await requestCode(page);
  await expect(otpPage.heading).toHaveText('Enter OTP');
  const configured = process.env.TEDILE_E2E_CUSTOMER_OTP.trim();
  expect(configured).toMatch(/^\d{4,6}$/);
  const last = configured.slice(-1);
  const invalid = configured.slice(0, -1) + (last === '9' ? '0' : String(Number(last) + 1));
  await otpPage.verify(invalid);
  await expect(otpPage.error).toHaveText('Invalid verification code.');
  expect(page.url()).toContain('/otp');
});

test('change-phone resets OTP challenge', async ({ page }) => {
  const { welcome, otpPage } = await requestCode(page);
  await otpPage.changePhoneButton.click();
  await page.waitForURL('**/');
  await expect(welcome.customerPhone).toBeVisible();
  await expect(welcome.customerPhone).toHaveValue('');
});
