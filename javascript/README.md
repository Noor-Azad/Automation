# Tedile Playwright JavaScript migration

The original C# + NUnit framework remains unchanged. JavaScript automation uses Playwright Test.

## Public tests (no credentials)

```bash
cd javascript
npm install
npx playwright install chromium
npx playwright test
```

## Authenticated tests (dedicated test account only)

The authenticated project is deliberately **opt-in**. Set these variables in your shell or CI secret store, not the repository:

- `TEDILE_E2E_CUSTOMER_PHONE`
- `TEDILE_E2E_CUSTOMER_OTP`
- `TEDILE_RUN_AUTHENTICATED=true`

Run with `npx playwright test`. Playwright executes `tests/auth.setup.js` **once**, saves browser storage state to ignored `playwright/.auth/customer.json`, and uses separate isolated contexts for the individual authenticated tests. When authenticated mode is enabled, worker count is limited to one to minimize shared-account interference.

`auth.setup.js` is excluded from normal public runs. Authentication state is sensitive: it is gitignored and must not be uploaded as CI artifacts.

**Production caution:** The dedicated review account can still be modified by logout and negative mutation tests. Prefer local/UAT with isolated seeded data for full authenticated regression. The C# suite is retained until all remaining scenarios and browsers have parity.

The separate JavaScript CI workflow currently runs safe public tests; it does not enable authenticated mode or replace production C# smoke.
