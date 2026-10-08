const { test, expect } = require('../../fixtures/auth');

test.describe.configure({ mode: 'serial' });

test('customer cannot open provider profile editor', async ({ authenticatedPage: page }) => {
  const response = await page.request.get('/provider/profile/edit', { maxRedirects: 0 });
  expect(response.status()).toBe(403);
  const body = await response.text();
  expect(body).not.toMatch(/Operational location|Save|Traceback/i);
});

test('customer cannot read provider working-hours owner endpoint', async ({ authenticatedPage: page }) => {
  const response = await page.request.get('/provider/working-hours', { maxRedirects: 0 });
  expect(response.status()).toBe(403);
  const body = await response.text();
  expect(body).not.toMatch(/days|configured|Traceback/i);
});
