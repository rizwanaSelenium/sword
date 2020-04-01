using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverContextMenu : WebDriverArmControl
    {
        public WebDriverContextMenu(IWebDriver driver, WebDriverWait waiter, string id) : base(driver, waiter, "div.context-menu#" + id)
        {
        }
    }
}
