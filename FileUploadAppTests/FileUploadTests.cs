using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System;
using System.IO;

namespace FileUploadAppTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }


        /*        
        This method writes an entry to log.txt file in the Results folder.    
        This is where all our test results are recorded.
        */

        private void WriteToLog(string message)
        {
            string currentDirectory = GetTestDirectory();
            string path =currentDirectory + @"\Results\log.txt";
            if (!File.Exists(path))
            {
                File.Create(path);
                TextWriter tw = new StreamWriter(path);
                tw.WriteLine(message);
                tw.Close();
            }
            else if (File.Exists(path))
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(System.DateTime.Now.ToString() + ":::" +  message);
                tw.Close();
            }
        }


       

       /*        
        This method gets the directory where we have Data and Results folders         
        */

        private string GetTestDirectory()
        {
            string currentDirectory = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            return currentDirectory;

        }



        /*        
        This is a test method that tests the use case of uploading a file to the web application 
        (success or failure)
        */

        [Test]
        public void TestFileUpload()
        {
            IWebDriver chromeDriver = new ChromeDriver();
            try
            {
                WriteToLog("INFORMATION : Test started");
                chromeDriver.Navigate().GoToUrl("http://the-internet.herokuapp.com/upload");
                Thread.Sleep(3000);

                // Test looks for the file upload button and it fails if the button is not found
                IWebElement chooseFileWebElement = chromeDriver.FindElement(By.Id("file-upload"));
                string currentDirectory = GetTestDirectory();

                // Upload the testFile.txt to the web application
                chooseFileWebElement.SendKeys(currentDirectory + @"\Data\testFile.txt");


                // Test looks for the file submit button and it fails if the button is not found            
                IWebElement fileUploadWebElement = chromeDriver.FindElement(By.Id("file-submit"));

                // Click the submit button
                fileUploadWebElement.Click();

                // Wait for 3 seconds
                Thread.Sleep(1000);

                // Test looks for the file submit button and it fails if the button is not found

                IWebElement fileUploadedElementSuccess = chromeDriver.FindElement(By.XPath("//*[text()=\"File Uploaded!\"]"));

                // Check whether the file uploaded message is shown. if shown, assume the file upload is successful
                if (fileUploadedElementSuccess.Displayed == true)
                {
                    WriteToLog("INFORMATION : File displayed message shown on the page. ");
                    WriteToLog("INFORMATION : Test passed");
                    chromeDriver.Quit();
                   
                }
                else
                {
                    WriteToLog("ERROR : The file upload has failed.");
                    WriteToLog("ERROR : File displayed message not found on the web page.");
                    Assert.Fail();
                    chromeDriver.Quit();
                }
            }

            catch (Exception e)
            {
                // Any exceptions are written to the log file 
                WriteToLog("EXCEPTION: " + e.Message.ToString());

                // Test is explicitly set to fail in case of exceptions
                Assert.Fail();
             
            }

            finally
            {
                // Close the browser window and end the test
                WriteToLog("Test ended.");
                chromeDriver.Quit();

            }

        }
    }
}