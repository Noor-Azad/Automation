# Tedile Playwright JavaScript migration (phase 1)

This folder is the JavaScript + `@playwright/test` equivalent of the initial C# Playwright + NUnit public UI and API tests.

## Start
```bash
cd javascript
npm install
npx playwright install chromium
npm test
```

Override the base URL via `TEDILE_BASE_URL=http://127.0.0.1:5001 npm test`.

Included: WelcomeTests, LegalTests, HealthApiTests, ProviderSearchContractTests.

This is **not yet** a complete conversion of the C# automation project. Existing authenticated customer, booking, security, Firefox/WebKit, and CI tests remain in C# until individually ported and verified. Do not remove or disable those suites.

No real OTP, secret, or production-data mutation is included in these tests. Prefer local/UAT for any test that creates users or bookings.

This branch intentionally does not modify the existing CI workflows, so it cannot disrupt the current production smoke automation.
