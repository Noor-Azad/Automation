# Tedile Automation Test Strategy

## Test layers

### 1. Public smoke
Runs on every PR and push:
- welcome/login shell
- legal pages
- health API
- security headers
- anonymous authorization boundaries
- safe malformed/injection-like input handling

### 2. Authenticated E2E
Runs when the dedicated Tedile review/test credentials are supplied:
- one real review-account OTP login
- Playwright storage-state reuse for subsequent tests
- customer navigation and future booking flows
- provider/admin flows will use dedicated test identities as they are added

### 3. Security regression
Automated tests are intentionally non-destructive:
- authentication/authorization boundaries
- CSRF and session checks
- security headers/CSP
- malformed input
- SQL-injection-like strings treated as invalid data
- sensitive-error leakage checks
- XSS/reflection checks as relevant fields are covered

Destructive scanning, broad fuzzing, credential attacks, or heavy traffic must not run against production.

### 4. Performance
k6 is kept separate from Playwright.

The checked-in public smoke profile defaults to only 2 virtual users for 20 seconds and is manual-only. Use UAT/staging for baseline, load, stress, spike, and soak testing.

## Planned E2E business flows

### Customer
- login
- location selection
- service search
- provider selection
- booking creation
- quote accept/counteroffer
- cancellation rules
- booking status visibility
- completion/review
- profile and notification flows

### Provider
- login
- dashboard
- availability/working hours
- incoming booking
- quote/counteroffer
- accept/reject
- on-the-way/location
- arrived/in-progress/completion
- cancellation and validation

### Admin
- login
- provider onboarding/approval
- provider blocking/reactivation
- service-management controls
- audit/security-sensitive operations

Each happy path will have negative/boundary coverage alongside it rather than being added only after the happy path is complete.
