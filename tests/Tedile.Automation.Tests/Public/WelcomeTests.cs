using Tedile.Automation.Core;
using Tedile.Automation.Pages;
using Tedile.Automation.TestData;
using static Microsoft.Playwright.Assertions;

namespace Tedile.Automation.Tests.Public;

public sealed class WelcomeTests : BaseTest
{
    [Test]
    public async Task Welcome_page_loads_with_customer_login_selected()
    {
        var welcome = new WelcomePage(Page);
        await welcome.OpenAsync();

        await Expect(welcome.Title).ToContainTextAsync("Local help");
        await Expect(welcome.CustomerForm).ToBeVisibleAsync();
        await Expect(welcome.ProviderForm).ToBeHiddenAsync();
    }

    [Test]
    public async Task Persona_toggle_switches_between_customer_and_provider_forms()
    {
        var welcome = new WelcomePage(Page);
        await welcome.OpenAsync();

        await welcome.SelectProviderAsync();
        await Expect(welcome.ProviderForm).ToBeVisibleAsync();
        await Expect(welcome.CustomerForm).ToBeHiddenAsync();

        await welcome.SelectCustomerAsync();
        await Expect(welcome.CustomerForm).ToBeVisibleAsync();
    }

        [TestCaseSource(typeof(TestDataProvider), nameof(TestDataProvider.InvalidCustomerPhones))]
    public async Task Invalid_customer_phone_is_rejected_without_entering_otp_flow(string phone)
    {
        var welcome = new WelcomePage(Page);
        await welcome.OpenAsync();
        await welcome.RequestCustomerOtpAsync(phone);

        await Expect(welcome.Error).ToBeVisibleAsync();
        Assert.DoesNotContain("/otp", Page.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Test]
    public async Task Alphabetic_phone_input_is_sanitized_and_native_required_validation_blocks_submit()
    {
        var welcome = new WelcomePage(Page);
        await welcome.OpenAsync();

        await welcome.CustomerPhone.FillAsync("abcdefghij");

        await Expect(welcome.CustomerPhone).ToHaveValueAsync(string.Empty);
        Assert.False(await welcome.CustomerPhone.EvaluateAsync<bool>("el => el.checkValidity()"));
        Assert.DoesNotContain("/otp", Page.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Test]
    public async Task Terms_and_privacy_links_point_to_public_legal_pages()
    {
        var welcome = new WelcomePage(Page);
        await welcome.OpenAsync();

        await Expect(welcome.TermsLink).ToHaveAttributeAsync("href", "/terms");
        await Expect(welcome.PrivacyLink).ToHaveAttributeAsync("href", "/privacy");
    }
}
