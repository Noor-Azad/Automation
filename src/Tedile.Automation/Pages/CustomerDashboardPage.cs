using Microsoft.Playwright;

namespace Tedile.Automation.Pages;

public sealed class CustomerDashboardPage : BasePage
{
    public CustomerDashboardPage(IPage page) : base(page) { }

    public ILocator HomeScreen => Page.Locator("#screen-home");
    public ILocator ServicesScreen => Page.Locator("#screen-services");
    public ILocator BookingsScreen => Page.Locator("#screen-bookings");
    public ILocator AccountScreen => Page.Locator("#screen-account");
    public ILocator ProfileEditScreen => Page.Locator("#screen-profile-edit");
    public ILocator NotificationsScreen => Page.Locator("#screen-notifications");
    public ILocator ServiceResultsScreen => Page.Locator("#screen-service-results");
    public ILocator ServiceFilter => Page.Locator("#service-filter");
    public ILocator ServiceCards => Page.Locator("#service-groups [data-service]");
    public ILocator ServiceSuggestions => Page.Locator("#service-suggestions [data-suggestion-service]");
    public ILocator ProviderResults => Page.Locator("#provider-results");
    public ILocator ResultsSubtitle => Page.Locator("#results-subtitle");
    public ILocator Greeting => Page.Locator("#customer-greeting");
    public ILocator BottomNav => Page.Locator("#customer-bottomnav");
    public ILocator LogoutButton => AccountScreen.Locator("form.logout-form button[type='submit']");
    public ILocator ProfilePhone => ProfileEditScreen.Locator("input[readonly]");

    public async Task OpenServicesFromHomeAsync()
    {
        await HomeScreen.Locator("button[data-screen='services']").First.ClickAsync();
    }

    public async Task BackFromServicesAsync()
    {
        await ServicesScreen.Locator("[data-customer-back='home']").ClickAsync();
    }

    public async Task WaitForServiceCatalogueAsync()
    {
        await ServiceCards.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }

    public async Task FilterServicesAsync(string value)
    {
        await ServiceFilter.FillAsync(value);
    }

    public async Task SelectFirstServiceAsync()
    {
        await ServiceCards.First.ClickAsync();
    }

    public async Task OpenBookingsAsync()
    {
        await BottomNav.Locator("button[data-screen='bookings']").ClickAsync();
    }

    public async Task OpenAccountAsync()
    {
        await BottomNav.Locator("button[data-screen='account']").ClickAsync();
    }

    public async Task OpenNotificationsFromAccountAsync()
    {
        await AccountScreen.Locator("button[data-screen='notifications']").ClickAsync();
    }

    public async Task OpenPersonalInformationAsync()
    {
        await AccountScreen.Locator("button[data-screen='profile-edit']").ClickAsync();
    }

    public async Task BackFromProfileEditAsync()
    {
        await ProfileEditScreen.Locator("[data-customer-back='account']").ClickAsync();
    }

    public async Task LogoutAsync()
    {
        await Page.RunAndWaitForNavigationAsync(
            async () => await LogoutButton.ClickAsync(),
            new PageRunAndWaitForNavigationOptions
            {
                WaitUntil = WaitUntilState.Load
            });
    }
}
