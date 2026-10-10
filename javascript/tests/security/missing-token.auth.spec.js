const { test, expect } = require('../../fixtures/auth');

test.describe.configure({ mode: 'serial' });

test('booking requests without form protection are rejected', async ({ authenticatedPage: page }) => {
  const result = await page.evaluate(async () => {
    const response = await fetch('/customer/bookings', {
      method: 'POST',
      credentials: 'same-origin',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded', Accept: 'application/json' },
      body: new URLSearchParams({
        provider_profile_code: 'AUTOMATION-NOT-A-REAL-PROVIDER',
        service_slug: 'automation-not-a-real-service'
      })
    });
    return { status: response.status, body: await response.text() };
  });
  expect(result.status).toBe(400);
  expect(result.body).not.toMatch(/Booking created|Traceback/i);
});

test('notification clearing without form protection is rejected', async ({ authenticatedPage: page }) => {
  const result = await page.evaluate(async () => {
    const response = await fetch('/notifications/clear', {
      method: 'POST',
      credentials: 'same-origin',
      headers: { Accept: 'application/json' }
    });
    return { status: response.status, body: await response.text() };
  });
  expect(result.status).toBe(400);
  expect(result.body).not.toMatch(/cleared|deleted|Traceback/i);
});
