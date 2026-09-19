using Microsoft.Playwright;

namespace Tedile.Automation.Pages;

public sealed class LegalPage : BasePage
{
    public LegalPage(IPage page) : base(page) { }

    public ILocator Heading => Page.Locator("h1");
    public ILocator Copyright => Page.GetByText("2026 Tedile. All rights reserved.", new() { Exact = false });
    public ILocator PrintButton => Page.GetByRole(AriaRole.Button, new() { Name = "Print / Save as PDF" });

    public async Task OpenAsync(string path)
    {
        await Page.GotoAsync(path);
        await WaitForReadyAsync();
    }
}
