using Microsoft.Playwright;
using Tedile.Automation.Core;
using Xunit.Abstractions;

namespace Tedile.Automation.Tests;

public abstract class AuthenticatedCustomerBaseTest : BaseTest
{
    protected AuthenticatedCustomerBaseTest(PlaywrightFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
    }

    protected override async Task<IBrowserContext> CreateContextAsync()
    {
        var storageState = await CustomerAuthStateManager.GetOrCreateAsync(Fixture);
        return await Fixture.CreateContextAsync(storageState);
    }
}
