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

## Confirmed Tedile UAT target

Authenticated mode now defaults to `https://tedile-uat.onrender.com` and refuses to target `https://tedile.in`. The public suite still defaults to production for read-only smoke checks.

For manual local UAT validation, use approved **UAT-only** credentials stored in your terminal environment:

```bash
cd javascript
TEDILE_RUN_AUTHENTICATED=true npx playwright test
```

The command requires `TEDILE_E2E_CUSTOMER_PHONE` and `TEDILE_E2E_CUSTOMER_OTP`. The shared authentication setup obtains a session once. OTP values must be valid at the time of execution.

A separate GitHub Actions workflow `Tedile UAT Authenticated Playwright` is prepared for **manual dispatch** and requires the repository secrets `TEDILE_UAT_CUSTOMER_PHONE` and `TEDILE_UAT_CUSTOMER_OTP`, plus a GitHub environment named `uat`. GitHub may not offer the dispatch control until the workflow is present on the default branch; keep the migration PR in draft until validation is arranged.

Before enabling authenticated tests, confirm the UAT application uses a separate test database and test messaging account; a different URL alone does **not** guarantee data isolation. Do not enable logout/session-mutation or additional OTP rate-limited suites during a normal regression run.
