using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverGroupPicker : WebDriverArmControl
    {
        public WebDriverGroupPicker(IWebDriver driver, WebDriverWait waiter, string id)
            : base(driver, waiter, "div#" + id)
        {
 
            SetSelectorString("div#" + id + " textarea");
        }
    }
}
