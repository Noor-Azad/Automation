#!/usr/bin/env bash
set -euo pipefail
# Deliberately UAT-only. This script never targets tedile.in.
export TEDILE_AUTH_MODE=uat
export TEDILE_BASE_URL=https://tedile-uat.onrender.com
export TEDILE_RUN_AUTHENTICATED=true
: "${TEDILE_E2E_CUSTOMER_PHONE:?Dedicated UAT customer phone required}"
: "${TEDILE_E2E_CUSTOMER_OTP:?Dedicated UAT test OTP required}"
unset TEDILE_RUN_SESSION_MUTATION TEDILE_RUN_OTP_RATE_LIMITED
echo "Checking UAT availability before authenticated regression..."
status=$(curl -sSL -o /dev/null --max-time 20 -w '%{http_code}' "$TEDILE_BASE_URL/" || true)
if [ "$status" != 200 ]; then
  echo "UAT unavailable: HTTP $status. No authenticated tests started."
  exit 2
fi
# Run the entire suite using the dedicated test account; ordinary logout and
# repeated OTP cases retain their separate explicit opt-in flags.
npx playwright test --project=chromium
