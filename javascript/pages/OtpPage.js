class OtpPage {
  constructor(page) {
    this.heading = page.locator('#otp-title');
    this.code = page.locator('#otp-code');
    this.verifyButton = page.getByRole('button', {name:'Verify code'});
    this.changePhoneButton = page.getByRole('button', {name:'Change phone number'});
    this.error = page.getByRole('alert');
  }
  async verify(otp) { await this.code.fill(otp); await this.verifyButton.click(); }
}
module.exports = { OtpPage };
