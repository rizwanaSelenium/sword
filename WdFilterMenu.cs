using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WdFilterMenu : WebDriverArmControl
    {
        public WdFilterMenu(IWebDriver driver, WebDriverWait waiter, string id) : base(driver, waiter, "div#" + id)
        {

        }

        public void SelectItemFromRootMenu(string rootMenuOption)
        {
            // Click "Please Select..." in Filter Menu to open the root menu
            var menus = Element.FindElements(By.CssSelector("div.dropDownTextBox"));
            var pleaseSelect = menus.Single(e => e.Text == "Please select...");
            pleaseSelect.Click();

            var rootMenus = Element.FindElements(By.CssSelector("div.ui-widget-content[key='cmroot']"));
            var rootMenu = rootMenus.Last();

            var rootMenuItems = rootMenu.FindElements(By.CssSelector("div.b-m-item"));

            var rootMenuItemFound = false;

            foreach (var rootMenuItem in rootMenuItems)
            {
                if (rootMenuItem.FindElement(By.CssSelector("span")).Text == rootMenuOption)
                {
                    // If the Menu Item text matches what we are searching for then we have found
                    // the item
                    rootMenuItemFound = true;
                    rootMenuItem.FindElement(By.CssSelector("div.b-m-arrow")).Click();
                    break;
                }
            }

            if (!rootMenuItemFound)
            {
                Assert.Fail("Filter Menu Root Item: " + rootMenuOption + " not found");
            }

        }

        public void SelectItemFromFirstSubMenu(string subMenuItem)
        {
            var firstSubMenu = Element.FindElement(By.CssSelector("div.ui-widget-content[key='0']"));
            
            var firstSubMenuItems = firstSubMenu.FindElements(By.CssSelector("div.b-m-item"));

            var firstSubMenuItemFound = false;

            foreach (var firstSubMenuItem in firstSubMenuItems)
            {
                if (firstSubMenuItem.FindElement(By.CssSelector("span")).Text == subMenuItem)
                {
                    // If the Menu Item text matches what we are searching for then we have found
                    // the item
                    firstSubMenuItemFound = true;
                    // Webdriver can't scroll through a DIV list and it also can't click on an element
                    // that is not visible, so click on the item using JavaScript just in case the
                    // option is not currently visible
                    var subMenuItemToClick = firstSubMenuItem.FindElement(By.CssSelector("div.b-m-ibody"));
                    Driver.ExecuteScript("arguments[0].click();", subMenuItemToClick);
                    break;
                }
            }

            if (!firstSubMenuItemFound)
            {
                Assert.Fail("Item from first Sub Menu: " + subMenuItem + " not found");
            }
        }

    }
}
