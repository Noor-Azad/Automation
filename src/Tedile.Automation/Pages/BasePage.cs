using Microsoft.Playwright;

namespace Tedile.Automation.Pages;

public abstract class BasePage
{
    protected BasePage(IPage page) => Page = page;

    protected IPage Page { get; }

    public Task<string> TitleAsync() => Page.TitleAsync();

    protected async Task WaitForReadyAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}
