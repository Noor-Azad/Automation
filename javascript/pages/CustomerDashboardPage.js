class CustomerDashboardPage {
  constructor(page) {
    this.page = page;
    this.homeScreen = page.locator('#screen-home');
    this.servicesScreen = page.locator('#screen-services');
    this.bookingsScreen = page.locator('#screen-bookings');
    this.accountScreen = page.locator('#screen-account');
    this.profileEditScreen = page.locator('#screen-profile-edit');
    this.notificationsScreen = page.locator('#screen-notifications');
    this.serviceResultsScreen = page.locator('#screen-service-results');
    this.serviceFilter = page.locator('#service-filter');
    this.serviceCards = page.locator('#service-groups [data-service]');
    this.serviceSuggestions = page.locator('#service-suggestions [data-suggestion-service]');
    this.providerResults = page.locator('#provider-results');
    this.resultsSubtitle = page.locator('#results-subtitle');
    this.greeting = page.locator('#customer-greeting');
    this.bottomNav = page.locator('#customer-bottomnav');
    this.profilePhone = this.profileEditScreen.locator('input[readonly]');
  }
  async openServicesFromHome() { await this.homeScreen.locator("button[data-screen='services']").first().click(); }
  async backFromServices() { await this.servicesScreen.locator("[data-customer-back='home']").click(); }
  async waitForServiceCatalogue() { await this.serviceCards.first().waitFor({state:'visible'}); }
  async filterServices(value) { await this.serviceFilter.fill(value); }
  async selectFirstService() { await this.serviceCards.first().click(); }
  async openBookings() { await this.bottomNav.locator("button[data-screen='bookings']").click(); }
  async openAccount() { await this.bottomNav.locator("button[data-screen='account']").click(); }
  async openNotificationsFromAccount() { await this.accountScreen.locator("button[data-screen='notifications']").click(); }
  async openPersonalInformation() { await this.accountScreen.locator("button[data-screen='profile-edit']").click(); }
  async backFromProfileEdit() { await this.profileEditScreen.locator("[data-customer-back='account']").click(); }
  async logout() { await Promise.all([this.page.waitForURL(url => !url.pathname.includes('/customer/dashboard')), this.accountScreen.locator("form.logout-form button[type='submit']").click()]); }
}
module.exports = { CustomerDashboardPage };
