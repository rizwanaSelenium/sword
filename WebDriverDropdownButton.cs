using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverDropdownButton : WebDriverButton
    {
        public WebDriverDropdownButton(IWebDriver driver, WebDriverWait waiter, string id)
            : base(driver, waiter, id)
        {
        }

        public T SelectOption<T>(string optionName) where T : WebDriverArmPage
        {
            SelectOption(optionName);
            return (T) Activator.CreateInstance(typeof(T), Driver, Waiter);
        }

        public void SelectOption(string optionName)
        {
            Element.Click();
            var listElements = Driver.FindElements(By.CssSelector("ul.menuButtonPopup li"));
            listElements.Single(li => li.Text == optionName).Click();
        }
    }
}
