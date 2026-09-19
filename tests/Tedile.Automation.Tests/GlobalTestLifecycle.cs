namespace Tedile.Automation.Tests;

[SetUpFixture]
public sealed class GlobalTestLifecycle
{
    [OneTimeSetUp]
    public async Task BeforeAllAsync()
    {
        await NUnitTestRuntime.Fixture.InitializeAsync();
    }

    [OneTimeTearDown]
    public async Task AfterAllAsync()
    {
        await NUnitTestRuntime.Fixture.DisposeAsync();
    }
}
