const { defineConfig, devices } = require('@playwright/test');
const authenticated = process.env.TEDILE_RUN_AUTHENTICATED === 'true';
const mode = process.env.TEDILE_AUTH_MODE || 'uat';
const productionSmoke = authenticated && mode === 'production-smoke';
const uat = 'https://tedile-uat.onrender.com';
const production = 'https://tedile.in';
if (authenticated && !['uat', 'production-smoke'].includes(mode)) {
  throw new Error('Unsupported authenticated mode');
}
const targetURL = process.env.TEDILE_BASE_URL || (productionSmoke ? production : authenticated ? uat : production);
if (authenticated && new URL(targetURL).origin !== new URL(productionSmoke ? production : uat).origin) {
  throw new Error('Authenticated target does not match selected UAT or production smoke mode');
}
const names = (process.env.TEDILE_BROWSERS || 'chromium').split(',').map(s => s.trim()).filter(Boolean);
const browsers = { chromium: devices['Desktop Chrome'], firefox: devices['Desktop Firefox'], webkit: devices['Desktop Safari'] };
for (const name of names) if (!browsers[name]) throw new Error('Unknown browser: ' + name);
const projectTests = names.map(name => ({
  name,
  testIgnore: ['**/auth.setup.js',
    ...(productionSmoke ? [
      '**/booking-*.auth.spec.js',
      '**/profile-validation.auth.spec.js',
      '**/mass-assignment.auth.spec.js',
      '**/customer-security.auth.spec.js',
      '**/logout*.auth.spec.js',
      '**/otp-*.auth.spec.js',
      '**/csp-otp-regression.spec.js',
      '**/missing-token.auth.spec.js',
      '**/notifications.auth.spec.js',
      '**/provider-status.auth.spec.js',
      '**/admin-matrix.auth.spec.js',
      '**/provider-admin-role.auth.spec.js',
      '**/customer-provider-owner.auth.spec.js',
    ] : []),
  ],
  use: { ...browsers[name] },
  ...(authenticated ? { dependencies: ['auth-setup'] } : {}),
}));
module.exports = defineConfig({
  testDir: './tests', timeout: 30_000, expect: { timeout: 10_000 },
  fullyParallel: true, forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  workers: authenticated ? 1 : process.env.CI ? 2 : 4,
  reporter: [['list'], ['html', { open: 'never' }]],
  use: {
    baseURL: targetURL,
    trace: productionSmoke ? 'off' : 'retain-on-failure',
    screenshot: productionSmoke ? 'off' : 'only-on-failure',
    actionTimeout: 10_000,
  },
  projects: authenticated ? [
    { name: 'auth-setup', testMatch: '**/auth.setup.js', use: { ...browsers.chromium } },
    ...projectTests,
  ] : projectTests,
});
