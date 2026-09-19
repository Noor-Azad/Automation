using Microsoft.Playwright;

namespace Tedile.Automation.Pages;

public sealed class ProviderProfilePage : BasePage
{
    public ProviderProfilePage(IPage page) : base(page) { }

    public ILocator Root => Page.Locator("#provider-profile");
    public ILocator Heading => Root.Locator(".provider-hero-body h1");
    public ILocator Location => Root.Locator(".provider-location");
    public ILocator Services => Root.Locator(".provider-service-card");
    public ILocator ReviewsSection => Root.Locator(".provider-profile-section").Filter(new() { HasText = "Reviews" });
    public ILocator RequestButton => Page.Locator("#start-booking");
    public ILocator BookingPanel => Page.Locator("#booking-panel");
    public ILocator BookingForm => Page.Locator("#booking-form");
    public ILocator PhoneField => BookingForm.Locator(".booking-phone-field input");
    public ILocator ServiceSelect => BookingForm.Locator("#booking-service");
    public ILocator ScheduledAt => BookingForm.Locator("input[name='scheduled_at']");
    public ILocator SubmitButton => BookingForm.GetByRole(AriaRole.Button, new() { Name = "Send booking request" });

    public async Task WaitForLoadedAsync()
    {
        await Heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }

    public async Task OpenBookingPanelAsync()
    {
        await RequestButton.ClickAsync();
        await BookingPanel.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }
}
