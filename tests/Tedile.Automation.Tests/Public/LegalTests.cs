using Tedile.Automation.Core;
using Tedile.Automation.Pages;
using Tedile.Automation.TestData;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;

namespace Tedile.Automation.Tests.Public;

public sealed class LegalTests : BaseTest, IClassFixture<PlaywrightFixture>
{
    public LegalTests(PlaywrightFixture fixture, ITestOutputHelper output) : base(fixture, output) { }

    [Theory]
    [MemberData(nameof(TestDataProvider.PublicLegalPages), MemberType = typeof(TestDataProvider))]
    public async Task Public_legal_pages_load_and_show_copyright(string path, string expectedHeading)
    {
        var legal = new LegalPage(Page);
        await legal.OpenAsync(path);

        await Expect(legal.Heading).ToContainTextAsync(expectedHeading);
        await Expect(legal.Copyright).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Provider_nda_print_button_calls_browser_print()
    {
        var legal = new LegalPage(Page);
        await legal.OpenAsync("/provider-nda");

        await Page.EvaluateAsync("window.__tedilePrintCalled = false; window.print = () => { window.__tedilePrintCalled = true; };");
        await legal.PrintButton.ClickAsync();

        var printCalled = await Page.EvaluateAsync<bool>("window.__tedilePrintCalled");
        Assert.True(printCalled, "The NDA Print / Save as PDF button should call window.print().");
    }
}
