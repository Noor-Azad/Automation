package pageObjects;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.FindBy;

public class AccountCreatedPage extends BasePage{
	
	public AccountCreatedPage(WebDriver driver)
	{
		super(driver);
	}

    @FindBy(css = "h2[data-qa='account-created']")
    private WebElement msgAccountCreated;

    @FindBy(css = "a[data-qa='continue-button']")
    private WebElement btnContinue;
  
    public WebElement getTextAccountCreated() {
        return msgAccountCreated;
    }

  
    public void continueButtonClick()
    {
    	btnContinue.click();
    }
    
}
