using Microsoft.Playwright;

namespace Tedile.Automation.Pages;

public sealed class OtpPage : BasePage
{
    public OtpPage(IPage page) : base(page) { }

    public ILocator Heading => Page.Locator("#otp-title");
    public ILocator Code => Page.Locator("#otp-code");
    public ILocator VerifyButton => Page.GetByRole(AriaRole.Button, new() { Name = "Verify code" });
    public ILocator ChangePhoneButton => Page.GetByRole(AriaRole.Button, new() { Name = "Change phone number" });

    public async Task VerifyAsync(string otp)
    {
        await Code.FillAsync(otp);
        await VerifyButton.ClickAsync();
    }
}
