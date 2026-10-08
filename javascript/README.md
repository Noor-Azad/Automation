# Tedile Playwright JavaScript migration

This folder introduces JavaScript Playwright Test alongside the existing C# Playwright + NUnit tests.

## Start
```bash
cd javascript
npm install
npx playwright install chromium
npm test
```

Included so far: public welcome/legal, health/provider-search API, public security/anonymous access, and gated authenticated customer navigation/session checks.

## Authenticated tests (opt-in only)

Only use a dedicated approved test/review account. Set `TEDILE_E2E_CUSTOMER_PHONE` and `TEDILE_E2E_CUSTOMER_OTP` outside source control and set `TEDILE_RUN_AUTHENTICATED=true` to enable. These tests are serial within their files and create OTP requests, which are rate-limited. Never run them against an ordinary user or uncontrolled production account. They skip by default.

Example (with environment variables already set):
```bash
TEDILE_RUN_AUTHENTICATED=true npx playwright test tests/customer --workers=1
```

Do **not** delete C# tests yet: full security/booking/role test parity, authenticated session reuse, cross-browser parity, and CI pipeline replacement are not done. The original workflow remains unchanged and is still the authoritative production smoke suite.
