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

        // Tedile's visible login field accepts the 10-digit national number only.
        // CI secrets may use the canonical +91XXXXXXXXXX form because Render's
        // GOOGLE_PLAY_REVIEW_PHONE requires that form. Strip +91 before typing
        // so welcome.js does not truncate the canonical number incorrectly.
        var loginPhone = ToLoginPhone(phone);
        await CustomerPhone.FillAsync(loginPhone);
        await CustomerSendCode.ClickAsync();
    }

    private static string ToLoginPhone(string phone)
    {
        var value = (phone ?? string.Empty).Trim();

        if (value.StartsWith("+91", StringComparison.Ordinal) &&
            value.Length == 13 &&
            value[3..].All(char.IsDigit))
        {
            return value[3..];
        }

        return value;
    }
}
