const { test, expect } = require('@playwright/test');
const { WelcomePage } = require('../../pages/WelcomePage');

test('welcome page loads with customer login selected', async ({ page }) => {
  const welcome = new WelcomePage(page);
  await welcome.open();
  await expect(welcome.title).toContainText('Local help');
  await expect(welcome.customerForm).toBeVisible();
  await expect(welcome.providerForm).toBeHidden();
});

test('persona toggle switches customer and provider', async ({ page }) => {
  const welcome = new WelcomePage(page);
  await welcome.open();
  await welcome.selectProvider();
  await expect(welcome.providerForm).toBeVisible();
  await expect(welcome.customerForm).toBeHidden();
  await welcome.selectCustomer();
  await expect(welcome.customerForm).toBeVisible();
});

for (const phone of ['12345', '0000000000']) {
  test(`invalid customer phone ${phone} does not enter OTP flow`, async ({ page }) => {
    const welcome = new WelcomePage(page);
    await welcome.open();
    await welcome.requestCustomerOtp(phone);
    await expect(welcome.error).toBeVisible();
    expect(page.url()).not.toMatch(/\/otp/i);
  });
}

test('alphabetic input sanitized and HTML validation blocks submit', async ({ page }) => {
  const welcome = new WelcomePage(page);
  await welcome.open();
  await welcome.customerPhone.fill('abcdefghij');
  await expect(welcome.customerPhone).toHaveValue('');
  expect(await welcome.customerPhone.evaluate(el => el.checkValidity())).toBe(false);
  expect(page.url()).not.toMatch(/\/otp/i);
});

test('terms and privacy links point to legal pages', async ({ page }) => {
  const welcome = new WelcomePage(page);
  await welcome.open();
  await expect(welcome.termsLink).toHaveAttribute('href', '/terms');
  await expect(welcome.privacyLink).toHaveAttribute('href', '/privacy');
});
