package pageObjects;

import java.io.IOException;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.FindBy;
import org.openqa.selenium.support.ui.Select;

import freemarker.core.ParseException;

public class EnterAccountInformationPage extends BasePage {

	public EnterAccountInformationPage(WebDriver driver) {
		super(driver);
	}

	@FindBy(xpath = "//b[contains(text(),'Enter Account Information')]")
	private WebElement msgEnterAccountInformation;

	@FindBy(id = "id_gender1")
	private WebElement radioTitleMr;

	@FindBy(id = "id_gender2")
	private WebElement radioTitleMrs;

	@FindBy(id = "password")
	private WebElement inputPassword;
	
	@FindBy(id = "days")
    private WebElement selectDays;

    @FindBy(id = "months")
    private WebElement selectMonths;

    @FindBy(id = "years")
    private WebElement selectYears;
    
    @FindBy(id = "newsletter")
    private WebElement checkboxNewsletter;

    @FindBy(id = "optin")
    private WebElement checkboxSpecialOffers;

    @FindBy(id = "first_name")
    private WebElement inputFirstName;

    @FindBy(id = "last_name")
    private WebElement inputLastName;

    @FindBy(id = "company")
    private WebElement inputCompany;

    @FindBy(id = "address1")
    private WebElement inputAddress1;

    @FindBy(id = "address2")
    private WebElement inputAddress2;

    @FindBy(id = "country")
    private WebElement selectCountry;

    @FindBy(id = "state")
    private WebElement inputState;

    @FindBy(id = "city")
    private WebElement inputCity;

    @FindBy(id = "zipcode")
    private WebElement inputZipcode;

    @FindBy(id = "mobile_number")
    private WebElement inputMobileNumber;

    @FindBy(css = "button[data-qa='create-account']")
    private WebElement btnCreateAccount;
    
    
    public WebElement msgEnterAccountInformation()
    {
    	return msgEnterAccountInformation;
    }
    
    public void fillAccountDetails() throws ParseException, IOException
    {
    	String password= "pass@123765";
    	radioTitleMr.click();
    	inputPassword.sendKeys(password);
    	Select days= new Select(selectDays);
    	days.selectByValue("1");
    	Select months= new Select(selectMonths);
    	months.selectByValue("1");
    	Select years= new Select(selectYears);
    	years.selectByValue("1996");
    	checkboxNewsletter.click();
    	checkboxSpecialOffers.click();
    	inputFirstName.sendKeys("Jyotirmoy");
    	inputLastName.sendKeys("Mandal");
    	inputCompany.sendKeys("Pierogi");
    	inputAddress1.sendKeys("1134 Columbia Road");
    	inputAddress2.sendKeys("Most");
    	Select countries = new Select(selectCountry);
    	countries.selectByValue("United States");
    	inputState.sendKeys("Texas");
    	inputCity.sendKeys("Dallas");
    	inputZipcode.sendKeys("98607");
    	inputMobileNumber.sendKeys("111222333");
    	btnCreateAccount.click();
    	
    }
    
}
