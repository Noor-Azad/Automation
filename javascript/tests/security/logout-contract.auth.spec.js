const { test, expect } = require('../../fixtures/auth');

test.describe.configure({ mode: 'serial' });
test.beforeEach(() => {
  test.skip(process.env.TEDILE_RUN_SESSION_MUTATION !== 'true',
    'Logout tests require isolated session-mutation execution');
});

test('GET logout is disallowed while retaining authentication', async ({ authenticatedPage: page }) => {
  const response = await page.request.get('/logout', { maxRedirects: 0 });
  expect(response.status()).toBe(405);
  const session = await page.request.get('/api/session');
  expect(session.status()).toBe(200);
});

test('POST logout without CSRF clears current session', async ({ authenticatedPage: page }) => {
  const before = await page.request.get('/api/session');
  expect(before.status()).toBe(200);
  const logoutStatus = await page.evaluate(async () => {
    const response = await fetch('/logout', { method: 'POST', credentials: 'same-origin' });
    return response.status;
  });
  expect(logoutStatus).toBe(200);
  const after = await page.request.get('/api/session');
  expect(after.status()).toBe(401);
  expect(await after.text()).not.toMatch(/phone|otp|password/i);
});
