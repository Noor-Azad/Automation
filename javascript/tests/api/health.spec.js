const { test, expect } = require('@playwright/test');

test('health endpoint reports Tedile as healthy', async ({ request }) => {
  const response = await request.get('/health', { headers: { Accept: 'application/json' } });
  expect(response.status()).toBe(200);
  const body = await response.json();
  expect(body.status).toBe('healthy');
  expect(body.app).toBe('Tedile');
});
