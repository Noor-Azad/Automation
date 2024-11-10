package testCases;

import java.io.IOException;

import org.testng.Assert;
import org.testng.annotations.Test;

import pageObjects.LoginSignupPage;
import testBase.BaseClass;

public class TestCase5 extends BaseClass{

	@Test
	public void registerUserWithExistingEmail() throws IOException
	{
		TestCase1.verifyThatHomepageIsVisibleSuccessfully();
		TestCase1.verifyNewUserSignupIsVisisble();
		verifyErrorEmailAddressAlreadyExistIsVisible();
	}
	
	private void verifyErrorEmailAddressAlreadyExistIsVisible() throws IOException
	{
		new LoginSignupPage(driver).fillIncorrectSignup();
		String emailAlreadyExistErrorText= new LoginSignupPage(driver).getErrorSignupText().getText();
		Assert.assertEquals(emailAlreadyExistErrorText, "Email Address already exist!", "Verify error 'Email Address already exist!' is visible");
	}
}
