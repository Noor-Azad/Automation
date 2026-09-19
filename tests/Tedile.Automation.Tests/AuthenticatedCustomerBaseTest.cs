using Microsoft.Playwright;
using Tedile.Automation.Core;

namespace Tedile.Automation.Tests;

public abstract class AuthenticatedCustomerBaseTest : BaseTest
{
    // Authenticated traces can contain sensitive application/session context.
    // Keep them out of CI artifacts by default.
    protected override bool CaptureTrace => false;

    protected override async Task<IBrowserContext> CreateContextAsync()
    {
        var storageState = await CustomerAuthStateManager.GetOrCreateAsync(Fixture);
        return await Fixture.CreateContextAsync(storageState);
    }
}
