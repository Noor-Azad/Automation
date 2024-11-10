package utilities;

import java.awt.Desktop;
import java.io.File;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;

import org.testng.ITestContext;
import org.testng.ITestListener;
import org.testng.ITestResult;

import com.aventstack.extentreports.ExtentReports;
import com.aventstack.extentreports.ExtentTest;
import com.aventstack.extentreports.Status;
import com.aventstack.extentreports.reporter.ExtentSparkReporter;
import com.aventstack.extentreports.reporter.configuration.Theme;

import testBase.BaseClass;

public class ExtentReportManager implements ITestListener {

    public ExtentSparkReporter sparkReporter;
    public ExtentReports extent;
    public ExtentTest test;

    String repName;
    private boolean isTestFailed = false; 

    public void onStart(ITestContext testContext) {

        String currentDateTimestamp = new SimpleDateFormat("yyyy-MM-dd_HH-mm-ss").format(new Date()); 

        repName = "Test-Report-" + currentDateTimestamp + ".html";
        sparkReporter = new ExtentSparkReporter(".\\reports\\" + repName); 
        sparkReporter.config().setTheme(Theme.DARK);

        extent = new ExtentReports();
        extent.attachReporter(sparkReporter);
        extent.setSystemInfo("User Name", System.getProperty("user.name"));
        extent.setSystemInfo("Environment", "QA");

    }

    public void onTestFailure(ITestResult result) {
        isTestFailed = true; 
        test = extent.createTest(result.getTestClass().getName());
//        test.assignCategory(result.getMethod().getGroups()); 

        test.log(Status.FAIL, result.getName() + " got failed");

        try {
            String imgPath = new BaseClass().captureScreen(result.getName());
            test.addScreenCaptureFromPath(imgPath);

        } catch (Exception e1) {
            e1.printStackTrace();
        }
    }

    public void onFinish(ITestContext testContext) {
        extent.flush();

        if (isTestFailed) { 
            String pathOfExtentReport = System.getProperty("user.dir") + "\\reports\\" + repName;
            File extentReport = new File(pathOfExtentReport);
            try {
                Desktop.getDesktop().browse(extentReport.toURI());
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }
}
