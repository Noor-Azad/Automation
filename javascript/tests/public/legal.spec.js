const { test, expect } = require('@playwright/test');
const { LegalPage } = require('../../pages/LegalPage');

for (const [path, heading] of [
  ['/terms', 'Terms of Service'],
  ['/privacy', 'Privacy Policy'],
  ['/provider-nda', 'Non-Disclosure Agreement'],
]) {
  test(`${path} shows heading and copyright`, async ({ page }) => {
    const legal = new LegalPage(page);
    await legal.open(path);
    await expect(legal.heading).toContainText(heading);
    await expect(legal.copyright).toBeVisible();
  });
}

test('provider NDA print button invokes browser print', async ({ page }) => {
  const legal = new LegalPage(page);
  await legal.open('/provider-nda');
  await page.evaluate(() => {
    window.__tedilePrintCalled = false;
    window.print = () => { window.__tedilePrintCalled = true; };
  });
  await legal.printButton.click();
  expect(await page.evaluate(() => window.__tedilePrintCalled)).toBe(true);
});
