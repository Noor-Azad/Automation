package testCases;

import org.testng.Assert;
import org.testng.annotations.Test;

import io.qameta.allure.Allure;
import io.qameta.allure.Description;
import io.qameta.allure.Epic;
import io.qameta.allure.Feature;
import io.qameta.allure.Owner;
import io.qameta.allure.Severity;
import io.qameta.allure.SeverityLevel;
import io.qameta.allure.Step;
import io.qameta.allure.Story;
import pageObjects.LoginSignupPage;
import testBase.BaseClass;

@Epic("Regression Tests")
@Feature("User")
public class TestCase3 extends BaseClass{
	
	
	@Test(testName="Test Case 3: Login User with incorrect email and password")
	@Description(""
			+ "1. Launch browser\r\n"
			+ "2. Navigate to url 'http://automationexercise.com'\r\n"
			+ "3. Verify that home page is visible successfully\r\n"
			+ "4. Click on 'Signup / Login' button\r\n"
			+ "5. Verify 'Login to your account' is visible\r\n"
			+ "6. Enter incorrect email address and password\r\n"
			+ "7. Click 'login' button\r\n"
			+ "8. Verify error 'Your email or password is incorrect!' is visible")
	@Owner("Jyotirmoy")
	@Severity(SeverityLevel.CRITICAL)
	@Story("Login User with incorrect email and password")
	public static void loginUserWithIncorrectEmailAndPassword()
	{
		Allure.label("tag", "Authentication");
		TestCase1.verifyThatHomepageIsVisibleSuccessfully();
		TestCase2.verifyLoginToYourAccountIsVisible();
		verifyErrorYourEmailOrPasswordIsIncorrectIsVisible();
		
	}

	@Step("Verify error 'Your email or password is incorrect!' is visible")
	public static void verifyErrorYourEmailOrPasswordIsIncorrectIsVisible()
	{
		String email= "incorrectUser@test.com";
		String password="incorrectPass";
		 new LoginSignupPage(driver).fillIncorrectLogin(email, password);
		 String errorText=new LoginSignupPage(driver).getErrorLoginText().getText();
		 Assert.assertEquals(errorText, "Your email or password is incorrect!", "Verify error 'Your email or password is incorrect!' is visible");
		 System.out.println("Your email or password is incorrect!");
	}
	
	
	
	
	
}
