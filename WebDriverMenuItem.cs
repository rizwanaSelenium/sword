using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverMenuItem : WebDriverArmControl
    {
        private readonly string _menuSelector;

        public WebDriverMenuItem(string id, IWebDriver driver, WebDriverWait waiter, string menuSelector)
            : base(driver, waiter, "li#" + id)
        {
            _menuSelector = menuSelector;
        }

        public void ClickOption(string optionName)
        {
            if (optionName == "Logout" || optionName == "Risk..." || optionName == "Secret Risk..." || optionName == "Amazing Opportunity...")
            {
                ClickOptionFromMenu(optionName);
                return;
            }
            Driver.ExecuteScript(string.Format(@"{0}.ClickMenuItem('{1}')", _menuSelector, optionName));
        }

        public void ClickOptionById(string idName)
        {
            Driver.ExecuteScript(string.Format(@"{0}.ClickMenuItemById('{1}')", _menuSelector, idName));
        }

        public void ClickOptionFromMenu(string optionName)
        {
            var options = Driver.FindElements(By.CssSelector("div#root-menu-div div.menu-item span"));
            foreach (var option in options)
            {
                if (option.Text == optionName)
                {
                    option.Click();
                    break;
                }
            }
        }

        public void AssertOptionVisible(MenuOption menuOption)
        {
            var visible = IsMenuItemByIdVisible(menuOption.GetEnumDescription());
            Assert.True(visible, "Expected menu option {0} to be visible but it was not.", menuOption.GetEnumDescription());
        }

        public void AssertOptionNotVisible(MenuOption menuOption)
        {
            var visible = IsMenuItemByIdVisible(menuOption.GetEnumDescription());
            Assert.False(visible, "Expected menu option {0} not to be visible but it was.", menuOption.GetEnumDescription());
        }

        public void AssertOptionEnabled(MenuOption menuOption)
        {
            var enabled = IsMenuItemByIdEnabled(menuOption.GetEnumDescription());
            Assert.True(enabled, "Expected menu option {0} to be enabled but it was not.", menuOption.GetEnumDescription());
        }

        public void AssertOptionNotEnabled(MenuOption menuOption)
        {
            var enabled = IsMenuItemByIdEnabled(menuOption.GetEnumDescription());
            Assert.False(enabled, "Expected menu option {0} not to be enabled but it was.", menuOption.GetEnumDescription());
        }

        private bool IsMenuItemByIdEnabled(string optionId)
        {
            var result = Driver.ExecuteScript(string.Format(@"return {0}.IsMenuItemByIdEnabled('{1}')", _menuSelector, optionId)).ToString();
            return Convert.ToBoolean(result);
        }

        private bool IsMenuItemByIdVisible(string optionId)
        {
            var result = Driver.ExecuteScript(string.Format(@"return {0}.IsMenuItemByIdVisible('{1}')", _menuSelector, optionId)).ToString();
            return Convert.ToBoolean(result);
        }


        public void ClickSubMenuOption(string optionName)
        {
            Driver.ExecuteScript(string.Format(@"{0}.ClickMenuItem('{1}')", _menuSelector, optionName));
        }
    }
}
