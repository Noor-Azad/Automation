package testCases;

import java.io.IOException;

import org.testng.Assert;
import org.testng.annotations.Test;

import io.qameta.allure.Epic;
import io.qameta.allure.Feature;
import pageObjects.LoggedHomePage;
import pageObjects.LoginSignupPage;
import testBase.BaseClass;

@Epic("Regression Tests")
@Feature("User")
public class TestCase4 extends BaseClass{

	@Test
	public void  logoutUser() throws IOException
	{
		TestCase1.verifyThatHomepageIsVisibleSuccessfully();
		TestCase2.verifyLoginToYourAccountIsVisible();
		TestCase2.verifyThatLoggedInAsUsernameIsVisible();
		verifyThatUserIsNavigatedToLoginPage();
		
	}
	
	public static void verifyThatUserIsNavigatedToLoginPage()
	{
		new LoggedHomePage(driver).clickLogoutButton();
		String loginToYourAccountText = new LoginSignupPage(driver).getTextLoginToAccount().getText();
		Assert.assertEquals(loginToYourAccountText, "Login to your account", "Verify 'Login to your account' is visible");
	}
	
}
