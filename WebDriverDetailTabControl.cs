using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class
        WebDriverDetailTabControl : WebDriverArmControl
    {
        public WebDriverDetailTabControl(IWebDriver driver, WebDriverWait waiter, string id)
            : base(driver, waiter, "div#" + id)
        {
            SetSelectorString("div#" + id + " ul");
        }

        public void SwitchTo(string tabName)
        {
            var tabs = Element.FindElements(By.CssSelector("li"));
            var tab = tabs.Single(t => t.Text == tabName);

            var tabLink = tab.FindElement(By.CssSelector("a"));
            tabLink.Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        public string ActiveTabName()
        {
            return Element.FindElement(By.CssSelector("li.ui-tabs-active a")).Text;
        }

        public bool AssertTabDisabled(string tabName)
        {
            var tabs = Element.FindElements(By.CssSelector("li"));
            if (tabs.Single(t => t.Text == tabName).GetAttribute("class").Contains("ui-state-disabled"))
            {
                return true;
            }
            return false;
        }

    }
}
