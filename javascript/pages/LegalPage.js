class LegalPage {
  constructor(page) {
    this.page = page;
    this.heading = page.locator('h1');
    this.copyright = page.getByText('2026 Tedile. All rights reserved.', { exact: false });
    this.printButton = page.getByRole('button', { name: 'Print / Save as PDF' });
  }
  async open(path) {
    await this.page.goto(path);
    await this.page.waitForLoadState('domcontentloaded');
  }
}
module.exports = { LegalPage };
