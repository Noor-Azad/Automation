package pageObjects;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.FindBy;

public class LoggedHomePage extends BasePage {
	
	public LoggedHomePage(WebDriver driver)
	{
		super(driver);
	}
	
	@FindBy(xpath="//li[contains(.,'Logged in as')]")
	private WebElement msgUsername;
	
	@FindBy(xpath="//li[contains(.,'Delete Account')]/a")
	private WebElement linkDeleteAccount;

	
	@FindBy(xpath="//li[contains(.,'Logout')]/a")
	private WebElement linkLogout;
	
	
	public WebElement getUsername()
	{
		return msgUsername;
	}

	public void clickLogoutButton()
	{
		linkLogout.click();
	}
	
	public void clickDeleteButton()
	{
		linkDeleteAccount.click();
	}
}
