const { test, expect } = require('../../fixtures/auth');

test.describe.configure({ mode: 'serial' });

test('unknown booking tracking returns clean not found response', async ({ authenticatedPage: page }) => {
  const response = await page.request.get('/customer/bookings/AUTOMATION-NOT-A-REAL-BOOKING/tracking');
  expect(response.status()).toBe(404);
  const body = await response.text();
  expect(JSON.parse(body).error).toBe('Booking not found');
  expect(body).not.toMatch(/latitude|longitude|Traceback/i);
});
