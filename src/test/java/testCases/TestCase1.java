package testCases;

import java.io.IOException;

import org.testng.Assert;
import org.testng.annotations.Test;

import freemarker.core.ParseException;
import io.qameta.allure.*;

import pageObjects.AccountCreatedPage;
import pageObjects.AccountDeletedPage;
import pageObjects.EnterAccountInformationPage;
import pageObjects.HomePage;
import pageObjects.LoggedHomePage;
import pageObjects.LoginSignupPage;
import testBase.BaseClass;

@Epic("Regression Tests")
@Feature("User")
public class TestCase1 extends BaseClass {
	
	public static String username="test6543";
	public static String password="Test90@test.com";
	
	
	@Test(description="Test Case 1: Register User", enabled = true)
	@Severity(SeverityLevel.CRITICAL)
	@Story("Register User")
	@Description(""
			+ "1. Launch browser\r\n"
			+ "2. Navigate to url 'http://automationexercise.com'\r\n"
			+ "3. Verify that home page is visible successfully\r\n"
			+ "4. Click on 'Signup / Login' button\r\n"
			+ "5. Verify 'New User Signup!' is visible\r\n"
			+ "6. Enter name and email address\r\n"
			+ "7. Click 'Signup' button\r\n"
			+ "8. Verify that 'ENTER ACCOUNT INFORMATION' is visible\r\n"
			+ "9. Fill details: Title, Name, Email, Password, Date of birth\r\n"
			+ "10. Select checkbox 'Sign up for our newsletter!'\r\n"
			+ "11. Select checkbox 'Receive special offers from our partners!'\r\n"
			+ "12. Fill details: First name, Last name, Company, Address, Address2, Country, State, City, Zipcode, Mobile Number\r\n"
			+ "13. Click 'Create Account button'\r\n"
			+ "14. Verify that 'ACCOUNT CREATED!' is visible\r\n"
			+ "15. Click 'Continue' button\r\n"
			+ "16. Verify that 'Logged in as username' is visible\r\n"
			+ "17. Click 'Delete Account' button\r\n"
			+ "18. Verify that 'ACCOUNT DELETED!' is visible and click 'Continue' button")
	public void registerUser() throws ParseException, IOException {
		verifyThatHomepageIsVisibleSuccessfully();
		verifyNewUserSignupIsVisisble();
		verifyThatEnterAccountInformationIsVisible();
		VerifyThatAccountCreatedIsVisible();
		verifyThatLoggedInAsUsernameIsVisible();
		verifyThatAccountDeletedIsVisibleAndClickContinueButton();
		

	}

	@Step("Verify that homepage is visible successfully")
	public static void verifyThatHomepageIsVisibleSuccessfully() {
		boolean homePageVisible= new HomePage(driver).homePageIsVisible().isDisplayed();
		Assert.assertTrue(homePageVisible,"Verify that homepage is visible successfully");
	}

	@Step("Verify 'New User Signup!' is visible")
	public static void verifyNewUserSignupIsVisisble() {
		new HomePage(driver).signupLoginClick();
		String newUserSignupText= new LoginSignupPage(driver).getNewUserSignup().getText();
		System.out.println(newUserSignupText);
		Assert.assertEquals(newUserSignupText, "New User Signup!", "verify 'New User Signup!' text heading is visible");
	}

	@Step("Verify that 'ENTER ACCOUNT INFORMTION' is visible")
	public static void verifyThatEnterAccountInformationIsVisible(){
		 new LoginSignupPage(driver).fillCorrectSignup(username, password);
		 String enterAccountInformtionText = new EnterAccountInformationPage(driver).msgEnterAccountInformation().getText();
		System.out.println(enterAccountInformtionText);
		Assert.assertEquals(enterAccountInformtionText, "ENTER ACCOUNT INFORMATION", "Verify that 'ENTER ACCOUNT INFORMATION' is visible");
	}
	

	
	@Step("Verify that 'ACCOUNT CREATED!' is visible")
	public static void VerifyThatAccountCreatedIsVisible() throws ParseException, IOException
	{
		 new EnterAccountInformationPage(driver).fillAccountDetails();
		 String accountCreatedText = new AccountCreatedPage(driver).getTextAccountCreated().getText();
		 System.out.println(accountCreatedText);
		Assert.assertEquals(accountCreatedText, "ACCOUNT CREATED!", "Verify that 'ACCOUNT CREATED!' is visible");
	}
	
	@Step("Verify that 'Logged in as username' is visible")
	public static void verifyThatLoggedInAsUsernameIsVisible()
	{
		 new AccountCreatedPage(driver).continueButtonClick();
		 
		 String loggedInAsUsernameText= new LoggedHomePage(driver).getUsername().getText();
		 System.out.println(loggedInAsUsernameText);
		Assert.assertEquals(loggedInAsUsernameText, "Logged in as "+username, "verify the text 'Logged in as username'");
		
		
	}

	@Step("Verify that 'ACCOUNT DELETED!' is visible and click 'Continue' button")
	public static void verifyThatAccountDeletedIsVisibleAndClickContinueButton()
	{
		
		new LoggedHomePage(driver).clickDeleteButton();
		String accountDeletedText = new AccountDeletedPage(driver).textAccountDeleted().getText();
		System.out.println(accountDeletedText);
		Assert.assertEquals(accountDeletedText, "ACCOUNT DELETED!", "Verify that 'ACCOUNT DELETED!' is visible");
		new AccountDeletedPage(driver).clickContinueBtn();
		
	}
}
