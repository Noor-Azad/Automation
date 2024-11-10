package testCases;

import java.io.IOException;

import org.testng.Assert;
import org.testng.annotations.Test;

import io.qameta.allure.Description;
import io.qameta.allure.Epic;
import io.qameta.allure.Feature;
import io.qameta.allure.Severity;
import io.qameta.allure.SeverityLevel;
import io.qameta.allure.Step;
import io.qameta.allure.Story;
import pageObjects.HomePage;
import pageObjects.LoggedHomePage;
import pageObjects.LoginSignupPage;
import testBase.BaseClass;

@Epic("Regression Tests")
@Feature("User")
public class TestCase2 extends BaseClass {

	@Test(description = "Test Case 2: Login User with correct email and password")
	@Severity(SeverityLevel.CRITICAL)
	@Story("Login User with correct email and password")
	@Description(""
			+ "1. Launch browser\r\n" 
			+ "2. Navigate to url 'http://automationexercise.com'\r\n"
			+ "3. Verify that home page is visible successfully\r\n" 
			+ "4. Click on 'Signup / Login' button\r\n"
			+ "5. Verify 'Login to your account' is visible\r\n" 
			+ "6. Enter correct email address and password\r\n"
			+ "7. Click 'login' button\r\n" 
			+ "8. Verify that 'Logged in as username' is visible\r\n")
	public static void loginUserWithCorrectEmailAndPassword() throws IOException {
		TestCase1.verifyThatHomepageIsVisibleSuccessfully();
		verifyLoginToYourAccountIsVisible();
		verifyThatLoggedInAsUsernameIsVisible();

	}

	@Step("Verify 'Login to your account' is visible")
	public static void verifyLoginToYourAccountIsVisible() {
		new HomePage(driver).signupLoginClick();
		String LoginToAccountText =new LoginSignupPage(driver).getTextLoginToAccount().getText();
		Assert.assertEquals(LoginToAccountText, "Login to your account", "Verify 'Login to your account' is visible");
	}

	@Step("Verify that 'Logged in as username' is visible")
	public static void verifyThatLoggedInAsUsernameIsVisible() throws IOException
	{
		String username="krone121";
		System.out.println(username);
		new LoginSignupPage(driver).fillCorrectLogin("kone@43241", "krone14321");
		String loggedInAsUsernameText= new LoggedHomePage(driver).getUsername().getText();
		Assert.assertEquals(loggedInAsUsernameText, "Logged in as "+username, "verify the text 'Logged in as username'");
	}
}
