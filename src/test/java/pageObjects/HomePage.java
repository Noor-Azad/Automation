package pageObjects;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.FindBy;

public class HomePage extends BasePage {
	
	
	public HomePage(WebDriver driver)
	{
		super(driver);
	}

	
	@FindBy(xpath="//div[@class='item active']//img[@alt='demo website for practice']")
	private WebElement imgHomePage;
	
	@FindBy(css="a[href='/login']")
	private WebElement btnSignupLogin;

	
	
	public WebElement homePageIsVisible()
	{
		return imgHomePage;
	}
	
	public void signupLoginClick()
	{
		btnSignupLogin.click();
	}

	
}
