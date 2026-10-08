const { defineConfig, devices } = require('@playwright/test');

const authenticated = process.env.TEDILE_RUN_AUTHENTICATED === 'true';
const browser = { ...devices['Desktop Chrome'] };
const browserNames = (process.env.TEDILE_BROWSERS || 'chromium')
  .split(',').map(value => value.trim()).filter(Boolean);
const browserDevices = {
  chromium: devices['Desktop Chrome'],
  firefox: devices['Desktop Firefox'],
  webkit: devices['Desktop Safari'],
};
for (const name of browserNames) {
  if (!browserDevices[name]) throw new Error('Unsupported TEDILE_BROWSERS entry: ' + name);
}
const requestedProjects = browserNames.map(name => ({
  name,
  testIgnore: '**/auth.setup.js',
  use: { ...browserDevices[name] },
  ...(authenticated ? { dependencies: ['auth-setup'] } : {}),
}));

module.exports = defineConfig({
  testDir: './tests',
  timeout: 30_000,
  expect: { timeout: 10_000 },
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  workers: authenticated ? 1 : (process.env.CI ? 2 : 4),
  reporter: [['list'], ['html', { open: 'never' }]],
  use: {
    baseURL: process.env.TEDILE_BASE_URL || 'https://tedile.in',
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
    actionTimeout: 10_000,
  },
  projects: authenticated ? [
    { name: 'auth-setup', testMatch: '**/auth.setup.js', use: browser },
    ...requestedProjects,
  ] : requestedProjects,
});
