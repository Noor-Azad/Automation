using Xunit;

namespace Tedile.Automation.Tests;

public sealed class RequiresCustomerCredentialsFactAttribute : FactAttribute
{
    public RequiresCustomerCredentialsFactAttribute()
    {
        var phone = Environment.GetEnvironmentVariable("TEDILE_E2E_CUSTOMER_PHONE");
        var otp = Environment.GetEnvironmentVariable("TEDILE_E2E_CUSTOMER_OTP");

        if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(otp))
        {
            Skip = "Set TEDILE_E2E_CUSTOMER_PHONE and TEDILE_E2E_CUSTOMER_OTP to run authenticated customer E2E tests.";
        }
    }
}
