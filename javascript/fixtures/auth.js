const { test: base, expect } = require('@playwright/test');
const path = require('node:path');

const authFile = path.join(__dirname, '..', 'playwright', '.auth', 'customer.json');

// Every test has its own isolated browser context. The one-time setup project
// writes authentication state before this suite starts. Never commit this file.
const test = base.extend({
  authenticatedPage: async ({ browser, baseURL }, use) => {
    base.skip(
      process.env.TEDILE_RUN_AUTHENTICATED !== 'true' ||
      !process.env.TEDILE_E2E_CUSTOMER_PHONE ||
      !process.env.TEDILE_E2E_CUSTOMER_OTP,
      'Dedicated account credentials and TEDILE_RUN_AUTHENTICATED=true required'
    );
    const context = await browser.newContext({ baseURL, storageState: authFile });
    try {
      const page = await context.newPage();
      await page.goto('/customer/dashboard');
      await expect(page).toHaveURL(/\/customer\/dashboard$/);
      await use(page);
    } finally {
      await context.close();
    }
  },
});
module.exports = { test, expect };
