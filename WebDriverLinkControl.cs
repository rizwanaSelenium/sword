using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
  public  class WebDriverLinkControl : WebDriverArmControl
    {
        public WebDriverLinkControl(IWebDriver driver, WebDriverWait waiter, string linkText)
            : base(driver, waiter, null)
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(500);
                if (Driver.FindElements(By.LinkText(linkText)).Count == 1)
                {
                    XPathString = "//a[contains(., '" + linkText + "')]";
                    break;
                }
                if (i == 9)
                {
                    //if we have still not got the button try to find it and fail with the normal messsage above is just giving it a good chance to find it
                    XPathString = "//a[contains(., '" + linkText + "')]";
                }
            }
        }

        public bool IsDisplayed()
        {
            return Element.Displayed;
        }

        public bool IsEnabled()
        {
            return Element.Enabled;
        }
    }
}
