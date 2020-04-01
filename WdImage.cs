using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WdImage : WebDriverArmControl
    {
        public WdImage(IWebDriver driver, WebDriverWait waiter) : base(driver, waiter, null)
        {
            SetSelectorString(".icon.sword-header-logo");
        }
    }
}
