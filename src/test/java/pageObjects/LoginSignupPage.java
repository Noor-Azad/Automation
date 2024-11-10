package pageObjects;

import java.io.FileNotFoundException;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.FindBy;

public class LoginSignupPage extends BasePage{
	

	
	public LoginSignupPage(WebDriver driver)
	{
		super(driver);
	}
	
	
	@FindBy(css="div[class='login-form'] h2")
	private WebElement msgLoginToYourAccount;
	
	@FindBy(css="input[data-qa='login-email']")
	private WebElement inputLoginEmail;
	
	@FindBy(css="input[data-qa='login-password']")
	private WebElement inputLoginPassword;
	
	@FindBy(css="button[data-qa=\"login-button\"]")
	private WebElement btnLogin;
		
	@FindBy(xpath="//p[contains(text(),'Your email or password is incorrect!')]")
	private WebElement msgErrorLogin;
	
	@FindBy(css="div[class='signup-form'] h2")
	private WebElement msgNewUserSignup;
	
	@FindBy(css="input[data-qa='signup-name']")
	private WebElement inputSignupName;
	
	@FindBy(css="input[data-qa='signup-email']")
	private WebElement inputSignupEmail;
	
	@FindBy(css="button[data-qa=\"signup-button\"]")
	private WebElement btnSignup;
	
	@FindBy(xpath="//p[contains(text(),'Email Address already exist!')]")
	private WebElement msgEmailAlreadyExist;

	
	
	
	
	public WebElement getNewUserSignup()
	{
		return msgNewUserSignup;
	}
	
	private void fillSignup(String name, String email)
	{
		inputSignupName.sendKeys(name);
		inputSignupEmail.sendKeys(email);
		btnSignup.click();
	}
	
	public void fillCorrectSignup(String name, String email)
	{
		fillSignup(name, email);
	}
	
	public void fillIncorrectSignup() throws FileNotFoundException
	{
		fillSignup("kroTest567", "kone@43241");
	}
	
	public WebElement getErrorSignupText()
	{
		return msgEmailAlreadyExist;
	}
	
	
	
	public WebElement getTextLoginToAccount()
	{
		return msgLoginToYourAccount;
	}
	
	private void fillLogin(String email, String password)
	{
		inputLoginEmail.sendKeys(email);
		inputLoginPassword.sendKeys(password);
		btnLogin.click();
	}
	
	public void fillCorrectLogin(String email, String password)
	{
		fillLogin(email, password);
	}
	
	public void fillIncorrectLogin(String email, String password)
	{
		fillLogin(email, password);
	}
	
	public WebElement getErrorLoginText()
	{
		return msgErrorLogin;
	}
	
}
