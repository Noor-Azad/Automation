# Tedile Playwright Automation

End-to-end automation for **Tedile** using **C# + Microsoft Playwright + xUnit**.

This repository intentionally replaces the previous Java/Selenium/TestNG proof-of-concept and its generated `target`, `test-output`, `allure-results`, and screenshot artifacts.

## Architecture

The framework follows the structure from the supplied C# automation reference screenshots, while adapting it to Playwright:

```text
Tedile.Automation.sln
├── src/Tedile.Automation
│   ├── Api                  # API clients
│   ├── Configuration        # TestSettings + ConfigReader
│   ├── Core                 # Playwright browser fixture + artifacts
│   ├── Models               # DTOs
│   ├── Pages                # Page Objects
│   ├── TestData             # MemberData / reusable test data
│   └── Utilities            # Logging helpers
├── tests/Tedile.Automation.Tests
│   ├── Api                  # API validation
│   ├── Customer             # Optional authenticated E2E flows
│   ├── Public               # Public smoke/regression UI tests
│   ├── BaseTest.cs          # Per-test context/page/trace lifecycle
│   └── testsettings.json
└── .github/workflows
    └── playwright.yml       # CI pipeline
```

### Design principles carried over from the reference framework

- central `BaseTest` lifecycle
- reusable driver/browser fixture (Playwright browser fixture)
- Page Object Model
- central configuration with environment-variable overrides
- reusable data provider
- logging through xUnit output
- API layer separate from UI pages
- execution artifacts (Playwright trace, screenshots/video when enabled)
- CI execution and downloadable test results

## Covered scenarios

Public tests run without credentials:

- Welcome page loads with customer login selected
- Customer/provider persona switching
- Invalid mobile-number validation without sending OTP
- Terms and Privacy links
- Terms, Privacy and Provider NDA pages
- Copyright validation
- Provider NDA Print / Save as PDF action invokes `window.print()`
- `/health` API returns healthy Tedile status

Authenticated customer tests are included but automatically skipped until a safe Tedile review/test account is configured:

- OTP customer login
- Customer dashboard load
- Services → Home deterministic Back navigation
- Account → Personal Information → Account deterministic Back navigation

## Local setup

Prerequisites:

- .NET 8 SDK
- PowerShell (`pwsh`) for the generated Playwright install script

```bash
dotnet restore Tedile.Automation.sln
dotnet build Tedile.Automation.sln
pwsh tests/Tedile.Automation.Tests/bin/Debug/net8.0/playwright.ps1 install

dotnet test Tedile.Automation.sln
```

Run headed Chromium:

```bash
PLAYWRIGHT_HEADLESS=false PLAYWRIGHT_BROWSER=chromium dotnet test Tedile.Automation.sln
```

Run WebKit to reproduce Safari/WebKit behavior:

```bash
PLAYWRIGHT_BROWSER=webkit dotnet test Tedile.Automation.sln
```

Run Firefox:

```bash
PLAYWRIGHT_BROWSER=firefox dotnet test Tedile.Automation.sln
```

## Configuration

Defaults are in `tests/Tedile.Automation.Tests/testsettings.json` and can be overridden with environment variables:

| Variable | Default | Purpose |
|---|---|---|
| `TEDILE_BASE_URL` | `https://tedile.in` | Target environment |
| `PLAYWRIGHT_BROWSER` | `chromium` | `chromium`, `firefox`, or `webkit` |
| `PLAYWRIGHT_HEADLESS` | `true` | Headless/headed run |
| `PLAYWRIGHT_SLOWMO_MS` | `0` | Debug slow motion |
| `PLAYWRIGHT_TIMEOUT_MS` | `30000` | Playwright timeout |
| `PLAYWRIGHT_TRACE` | `true` | Save trace ZIPs |
| `PLAYWRIGHT_VIDEO` | `false` | Record video |
| `TEDILE_E2E_CUSTOMER_PHONE` | blank | Optional review/test customer |
| `TEDILE_E2E_CUSTOMER_OTP` | blank | Optional fixed review/test OTP |

Never commit real OTPs, passwords, private keys or production secrets.

## GitHub Actions

`.github/workflows/playwright.yml` runs on PRs and pushes to `main`.

The pipeline:

1. restores .NET dependencies
2. builds the solution
3. installs Playwright Chromium
4. runs xUnit + Playwright tests
5. uploads TRX results and Playwright trace artifacts

To enable authenticated customer tests in CI, add these **GitHub repository secrets**:

- `TEDILE_E2E_CUSTOMER_PHONE`
- `TEDILE_E2E_CUSTOMER_OTP`

Until those secrets exist, authenticated tests are reported as skipped rather than failing or sending OTPs to arbitrary phone numbers.

## Debugging failures

Traces are written under `artifacts/traces`.

Open a trace locally with:

```bash
pwsh tests/Tedile.Automation.Tests/bin/Debug/net8.0/playwright.ps1 show-trace artifacts/traces/<trace>.zip
```

CI uploads the same trace files as downloadable GitHub Action artifacts.
