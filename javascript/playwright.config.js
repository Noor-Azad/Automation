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
const safeProductionFiles = [
  '**/api/health.spec.js',
  '**/api/provider-search.spec.js',
  '**/public/legal.spec.js',
  '**/public/welcome.spec.js',
  '**/security/public-security.spec.js',
  '**/security/public-provider-privacy.spec.js',
  '**/security/cache-public.spec.js',
  '**/security/search-robustness.spec.js',
  '**/security/session-privacy.auth.spec.js',
  '**/customer/customer-session.auth.spec.js',
  '**/customer/session.auth.spec.js',
  '**/security/csp.auth.spec.js',
  '**/security/customer-provider-owner.auth.spec.js',
  '**/security/headers.auth.spec.js',
  '**/customer/service-discovery.auth.spec.js',
  '**/customer/provider-profile.auth.spec.js',
  '**/customer/navigation.auth.spec.js',
  '**/customer/primary-sections.auth.spec.js',
];
const projectTests = names.map(name => ({
  name,
  ...(productionSmoke ? { testMatch: safeProductionFiles } : {}),
  testIgnore: '**/auth.setup.js',
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
