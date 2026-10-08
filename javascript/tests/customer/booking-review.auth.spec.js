const { test, expect } = require('../../fixtures/auth');

test.describe.configure({ mode: 'serial' });

test('unknown booking review returns not found with valid form token', async ({ authenticatedPage: page }) => {
  const result = await page.evaluate(async () => {
    const csrf = document.querySelector('meta[name="csrf-token"]')?.content || '';
    const response = await fetch('/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/review', {
      method: 'POST',
      credentials: 'same-origin',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/x-www-form-urlencoded',
        'X-CSRFToken': csrf
      },
      body: new URLSearchParams({ rating: '5', comment: 'automation-boundary-check' })
    });
    return { status: response.status, body: await response.text() };
  });

  expect(result.status).toBe(404);
  expect(result.body).toContain('Booking not found');
  expect(result.body).not.toMatch(/Traceback/i);
});
