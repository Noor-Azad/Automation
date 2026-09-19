using Microsoft.Playwright;

namespace Tedile.Automation.Pages;

public sealed class CustomerDashboardPage : BasePage
{
    public CustomerDashboardPage(IPage page) : base(page) { }

    public ILocator HomeScreen => Page.Locator("#screen-home");
    public ILocator ServicesScreen => Page.Locator("#screen-services");
    public ILocator AccountScreen => Page.Locator("#screen-account");
    public ILocator ProfileEditScreen => Page.Locator("#screen-profile-edit");
    public ILocator Greeting => Page.Locator("#customer-greeting");

    public async Task OpenServicesFromHomeAsync()
    {
        await HomeScreen.Locator("button[data-screen='services']").First.ClickAsync();
    }

    public async Task BackFromServicesAsync()
    {
        await ServicesScreen.Locator("[data-customer-back='home']").ClickAsync();
    }

    public async Task OpenAccountAsync()
    {
        await Page.Locator("#customer-bottomnav button[data-screen='account']").ClickAsync();
    }

    public async Task OpenPersonalInformationAsync()
    {
        await AccountScreen.Locator("button[data-screen='profile-edit']").ClickAsync();
    }

    public async Task BackFromProfileEditAsync()
    {
        await ProfileEditScreen.Locator("[data-customer-back='account']").ClickAsync();
    }
}
