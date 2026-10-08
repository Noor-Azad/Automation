# C# → JavaScript migration coverage and validation

The source C# + NUnit project remains in this repository as the baseline and fallback. The migration branch contains JavaScript Playwright Test translations.

## Scenario mapping

| Original C# suites | JavaScript test locations |
| --- | --- |
| Public WelcomeTests, LegalTests | tests/public |
| HealthApiTests, ProviderSearchContractTests | tests/api |
| Customer authenticated smoke/navigation and service discovery | tests/customer/primary-sections.auth.spec.js, navigation.auth.spec.js, service-discovery.auth.spec.js |
| CustomerAuthenticationTests | tests/auth.setup.js (shared login prerequisite) |
| CustomerAuthenticationNegativeTests | tests/customer/otp-regression.spec.js (separate rate-limited opt-in) |
| CustomerBookingApiValidationTests, CustomerBookingValidationTests | tests/customer/booking-api-validation.auth.spec.js, booking-ui-validation.auth.spec.js |
| CustomerProviderProfileTests, CustomerSessionApiTests | tests/customer/provider-profile.auth.spec.js, customer-session.auth.spec.js |
| PublicSecurity, anonymous access/mutation, public provider privacy, query robustness | tests/security/public-security.spec.js, anonymous-access.spec.js, anonymous-mutation.spec.js, public-provider-privacy.spec.js, search-robustness.spec.js |
| Admin and provider owner/role boundaries | tests/security/admin-matrix.auth.spec.js, provider-admin-role.auth.spec.js, customer-provider-owner.auth.spec.js, provider-status.auth.spec.js, role-boundary.auth.spec.js |
| Booking ownership and CSRF boundaries | tests/customer/booking-review.auth.spec.js, booking-tracking.auth.spec.js, tests/security/booking-ownership-review.auth.spec.js, missing-token.auth.spec.js |
| Profile validation and mass assignment | tests/security/profile-validation.auth.spec.js, customer-security.auth.spec.js, mass-assignment.auth.spec.js |
| Security headers, session privacy, cache, cookies, CSP, fixation | tests/security/headers.auth.spec.js, session-privacy.auth.spec.js, session.auth.spec.js, cache-public.spec.js, csp.auth.spec.js, csp-otp-regression.spec.js, otp-login-rotation.auth.spec.js |
| NotificationSecurityBoundaryTests | tests/security/notifications.auth.spec.js |
| LogoutSessionInvalidationTests and LogoutPostContractTests | tests/customer/logout.auth.spec.js, tests/security/logout-contract.auth.spec.js |

## Verification

- **Confirmed locally by project owner prior to recent commits:** 49 passed and 55 skipped in the public run.
- **Recent changes:** not yet execution-verified. GitHub CI now includes Node syntax checking and public Playwright testing on pull requests.
- **Critical authenticated coverage:** default-skipped because credentials and approved environment are required. Skipped is not passed.
- **Cross-browser parity:** opt-in `TEDILE_BROWSERS=chromium,firefox,webkit`; not yet execution-verified.
- **End-to-end production mutations:** deliberately not enabled in the safe default smoke. Full booking lifecycle should use isolated UAT fixtures, not live customer bookings.

## How to run

```bash
cd javascript
npm install
npx playwright install chromium
npx playwright test
# optional cross-browser smoke
TEDILE_BROWSERS=chromium,firefox,webkit npx playwright test tests/public
```

For authenticated tests, provide `TEDILE_E2E_CUSTOMER_PHONE`, `TEDILE_E2E_CUSTOMER_OTP`, and `TEDILE_RUN_AUTHENTICATED=true`. The auth setup requests one OTP and saves a gitignored browser state. Rate-limited OTP tests need a separate `TEDILE_RUN_OTP_RATE_LIMITED=true` opt-in. Session-mutating logout tests need `TEDILE_RUN_SESSION_MUTATION=true` and an isolated UAT account.

## Migration completion gate

Keep the original C# suite and existing C# production CI until:

1. JavaScript public suite and syntax checks pass on the final commit.
2. Authenticated suite passes with a dedicated UAT test account.
3. Cross-browser smoke passes on required browsers.
4. Full booking lifecycle has dedicated seeded UAT data and demonstrably equivalent coverage.
5. The new JavaScript CI workflow is enabled and validated, and old C# CI is retired by a reviewed change.

**Do not treat test file counts or skipped test counts as proof of full parity.**
