const { test, expect } = require('@playwright/test');
const { mkdir } = require('node:fs/promises');
const path = require('node:path');

const authFile = path.join(__dirname, '..', 'playwright', '.auth', 'customer.json');

function csrfFrom(html) {
  const meta = html.match(/<meta\s+name=["']csrf-token["']\s+content=["']([^"']+)["']/i);
  const input = html.match(/<input[^>]*name=["']csrf_token["'][^>]*value=["']([^"']+)["'][^>]*>/i);
  if (!meta && !input) throw new Error('CSRF token absent from Tedile authentication form');
  return (meta || input)[1];
}

test('login dedicated review customer once using C#-equivalent API bootstrap', async ({ page }) => {
  test.setTimeout(180_000);
  const phone = process.env.TEDILE_E2E_CUSTOMER_PHONE;
  const otp = process.env.TEDILE_E2E_CUSTOMER_OTP;
  expect(phone, 'Dedicated review/test phone is required').toBeTruthy();
  expect(otp, 'Dedicated review/test OTP is required').toBeTruthy();
  const request = page.request;

  const welcome = await request.get('/', { timeout: 90_000 });
  expect(welcome.status(), 'Welcome endpoint unavailable').toBe(200);
  const login = await request.post('/customer/login', {
    form: { csrf_token: csrfFrom(await welcome.text()), phone },
    maxRedirects: 0,
  });
  expect(login.status(), 'Customer OTP challenge rejected').toBe(302);

  const otpPage = await request.get('/otp');
  expect(otpPage.status()).toBe(200);
  const verified = await request.post('/otp/verify', {
    form: { csrf_token: csrfFrom(await otpPage.text()), otp },
    maxRedirects: 0,
  });
  expect(verified.status(), 'Customer OTP verification rejected').toBe(302);
  const session = await request.get('/api/session');
  expect(session.status()).toBe(200);
  expect((await session.json()).authenticated).toBe(true);
  await mkdir(path.dirname(authFile), { recursive: true });
  await page.context().storageState({ path: authFile });
});
