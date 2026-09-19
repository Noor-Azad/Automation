using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Tedile.Automation.Tests;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequiresCustomerCredentialsAttribute : NUnitAttribute, IApplyToTest
{
    public void ApplyToTest(Test test)
    {
        var phone = Environment.GetEnvironmentVariable("TEDILE_E2E_CUSTOMER_PHONE");
        var otp = Environment.GetEnvironmentVariable("TEDILE_E2E_CUSTOMER_OTP");

        if (!string.IsNullOrWhiteSpace(phone) && !string.IsNullOrWhiteSpace(otp))
        {
            return;
        }

        test.RunState = RunState.Ignored;
        test.Properties.Set(
            PropertyNames.SkipReason,
            "Set TEDILE_E2E_CUSTOMER_PHONE and TEDILE_E2E_CUSTOMER_OTP to run authenticated customer E2E tests.");
    }
}
