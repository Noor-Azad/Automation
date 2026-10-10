const { test, expect } = require('../../fixtures/auth');

test.describe.configure({ mode: 'serial' });

test('customer cannot update a provider booking status', async ({ authenticatedPage: page }) => {
  const result = await page.evaluate(async () => {
    const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
    const response = await fetch('/provider/bookings/AUTOMATION-NOT-A-REAL-BOOKING/status', {
      method: 'POST',
      credentials: 'same-origin',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/x-www-form-urlencoded',
        'X-CSRFToken': csrf
      },
      body: new URLSearchParams({ status: 'completed' })
    });
    return { status: response.status, body: await response.text() };
  });
  expect(result.status).toBe(403);
  expect(result.body).not.toMatch(/completed|updated|Traceback/i);
});
