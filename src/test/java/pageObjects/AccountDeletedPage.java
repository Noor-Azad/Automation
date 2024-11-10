package pageObjects;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.FindBy;

public class AccountDeletedPage extends BasePage{

	public AccountDeletedPage(WebDriver driver)
	{
		super(driver);
	}

	@FindBy(xpath="//h2[contains(.,'Account Deleted!')]")
	private WebElement msgAccountDeleted;
	
	@FindBy(xpath="//a[@data-qa=\"continue-button\"]")
	private WebElement btnContinue;
	
	
	public WebElement textAccountDeleted()
	{
		return msgAccountDeleted;
	}
	
	public HomePage clickContinueBtn()
	{
		btnContinue.click();
		return new HomePage(driver);
	}
}
