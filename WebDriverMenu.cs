using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverMenu : WebDriverArmControl
    {
        private const string MainMenuSelector = "window.RootView.Menu";

        public WebDriverMenu(IWebDriver driver, WebDriverWait waiter, string selector) : base(driver, waiter, "ul#" + selector)
        {
        }

        public WebDriverMenuItem GetMenuItemById(string id)
        {
            return new WebDriverMenuItem(id, Driver, Waiter, MainMenuSelector);
        }
    }
}
