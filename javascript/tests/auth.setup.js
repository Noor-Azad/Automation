const { test, expect } = require('@playwright/test');
const { mkdir } = require('node:fs/promises');
const path = require('node:path');
const { WelcomePage } = require('../pages/WelcomePage');
const { OtpPage } = require('../pages/OtpPage');

const authFile = path.join(__dirname, '..', 'playwright', '.auth', 'customer.json');

test('login dedicated review customer once', async ({ page }, testInfo) => {
  test.setTimeout(150_000);
  const phone = process.env.TEDILE_E2E_CUSTOMER_PHONE;
  const code = process.env.TEDILE_E2E_CUSTOMER_OTP;
  expect(phone, 'Missing dedicated test phone').toBeTruthy();
  expect(code, 'Missing dedicated test OTP').toBeTruthy();

  const welcome = new WelcomePage(page);
  // Render's free UAT instance may need over 50 seconds to wake after inactivity.
  // Confirm the actual login screen before making any OTP request.
  let ready = false;
  let lastStatus = 'no response';
  for (let attempt = 1; attempt <= 3 && !ready; attempt++) {
    const response = await page.goto('/', { waitUntil: 'domcontentloaded', timeout: 75_000 });
    lastStatus = response ? String(response.status()) : 'no response';
    try {
      await welcome.customerTab.waitFor({ state: 'visible', timeout: 20_000 });
      ready = true;
    } catch {
      if (attempt < 3) await page.waitForTimeout(2_000);
    }
  }
  if (!ready) {
    const details = await page.locator('body').innerText({ timeout: 5_000 }).catch(() => '(body unavailable)');
    const diagnostic = [
      'UAT login screen was not found after wake-up retries.',
      'URL: ' + page.url(),
      'HTTP status: ' + lastStatus,
      'Page title: ' + await page.title().catch(() => '(unavailable)'),
      'Page body excerpt: ' + details.slice(0, 600)
    ].join('\\n');
    await testInfo.attach('uat-login-diagnostic', { body: diagnostic, contentType: 'text/plain' });
    throw new Error(diagnostic);
  }
  await welcome.requestCustomerOtp(phone);
  await page.waitForURL('**/otp');
  await new OtpPage(page).verify(code);
  await page.waitForURL('**/customer/dashboard');
  await expect(page.locator('#screen-home')).toHaveClass(/active/);

  await mkdir(path.dirname(authFile), { recursive: true });
  await page.context().storageState({ path: authFile });
});
