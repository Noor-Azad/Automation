using Microsoft.Playwright;

namespace Tedile.Automation.Pages;

public sealed class WelcomePage : BasePage
{
    public WelcomePage(IPage page) : base(page) { }

    public ILocator Title => Page.Locator("#welcome-title");
    public ILocator CustomerTab => Page.Locator("#persona-customer");
    public ILocator ProviderTab => Page.Locator("#persona-provider");
    public ILocator CustomerForm => Page.Locator("[data-auth-form='customer-login']");
    public ILocator ProviderForm => Page.Locator("[data-auth-form='provider-login']");
    public ILocator CustomerPhone => Page.Locator("#customer-login-phone");
    public ILocator CustomerSendCode => CustomerForm.GetByRole(AriaRole.Button, new() { Name = "Send code" });
    public ILocator Error => Page.GetByRole(AriaRole.Alert);
    public ILocator TermsLink => Page.GetByRole(AriaRole.Link, new() { Name = "Terms of Service" });
    public ILocator PrivacyLink => Page.GetByRole(AriaRole.Link, new() { Name = "Privacy Policy" });

    public async Task OpenAsync()
    {
        await Page.GotoAsync("/");
        await WaitForReadyAsync();
    }

    public async Task SelectCustomerAsync() => await CustomerTab.ClickAsync();
    public async Task SelectProviderAsync() => await ProviderTab.ClickAsync();

    public async Task RequestCustomerOtpAsync(string phone)
    {
        await SelectCustomerAsync();
        await CustomerPhone.FillAsync(phone);
        await CustomerSendCode.ClickAsync();
    }
}
