# Tedile JavaScript Playwright Automation

End-to-end browser, API and security regression coverage for **https://tedile.in** using **JavaScript + Playwright Test**.

## Quick start

```bash
cd javascript
npm install
npx playwright install chromium
npx playwright test
```

The default suite runs public tests and skips tests that require the dedicated review account. CI validates the public suite with Chromium, Firefox and WebKit.

## Dedicated production review-account smoke

The manual workflow **Tedile Production JavaScript Auth Smoke** uses these GitHub secrets:

- `TEDILE_E2E_CUSTOMER_PHONE`
- `TEDILE_E2E_CUSTOMER_OTP`

Only an explicitly configured, reviewed allowlist of safe production tests runs. The `auth-setup` project establishes a session using Tedile's normal OTP verification, and each test uses an isolated browser context. Never upload the session state or place secrets in Git.

Tests with booking/profile writes, logout, or repeated OTP attempts remain in `javascript/tests` for controlled isolated-environment execution. **Passing the production smoke does not certify those excluded cases.** The production config intentionally blocks broad authenticated execution.

## Source layout

- `javascript/tests/`: Playwright test specs and authentication setup
- `javascript/pages/`: Page Object Model
- `javascript/fixtures/`: isolated authenticated page fixtures
- `javascript/utils/`: shared test utilities
- `javascript/playwright.config.js`: browsers, environment restrictions, and test selection
- `.github/workflows/playwright-javascript.yml`: JavaScript Chromium/Firefox/WebKit CI
- `.github/workflows/production-auth-smoke-javascript.yml`: manual protected production smoke
- `performance/k6/`: independent performance smoke

The legacy C# / NUnit automation framework was retired from this branch after the JavaScript cutover. Historical source remains available in Git history.
