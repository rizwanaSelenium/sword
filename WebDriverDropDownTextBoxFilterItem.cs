using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverDropDownTextBoxFilterItem : WebDriverArmControl
    {
        public WebDriverDropDownTextBoxFilterItem(IWebDriver driver, WebDriverWait waiter, string selector)
            : base(driver, waiter, "tr.filter-criteria td." + selector)
        {
          
        }

        public void SelectItemFromRootMenu(string option)
        {
            var elements = Driver.FindElements(By.CssSelector("tr.filter-criteria td.property div.dropDownTextBox"));

            if (!elements.Any())
                Assert.Fail("Could not find any items in the list '{0}'", CssSelectorString);

            var filterMenuDropDown = elements.Single(e => e.Text == "Please select...");
            filterMenuDropDown.Click();

            const string rootMenuSelector = "div#filterMenu div.b-m-mpanel[key='cmroot']";
            var rootMenus = Driver.FindElements(By.CssSelector(rootMenuSelector));
            if (!rootMenus.Any())
                Assert.Fail("Could not find any items in the list '{0}'", rootMenuSelector);

            var rootMenu = rootMenus.Last();
            var rootMenuItem = rootMenu.FindElement(By.CssSelector("div[title='" + option + "']"));
            rootMenuItem.Click();
        }

        public void SelectItemFromFirstSubMenu(string option)
        {
            const string selector = "div#filterMenu div.b-m-mpanel[key='0']";
            var subMenus = Driver.FindElements(By.CssSelector(selector));

            if(!subMenus.Any())
                Assert.Fail("Could not find any items in the list '{0}'", selector);

            var subMenu = subMenus.Last();
            var firstSubMenuItem = subMenu.FindElement(By.CssSelector("div[title='" + option + "']"));
            firstSubMenuItem.Click();
        }
    }
}
