using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverDropDownTextBox : WebDriverArmControl
    {
        public WebDriverDropDownTextBox(IWebDriver driver, WebDriverWait waiter, string selector) : base(driver, waiter, "div.armcontrol " + selector)
        {
         
        }

        public void ClickElement()
        {
            Element.Click();
            WaitForElementToAppear();
        }

        public void SelectValue(string filter)
        {
            string[] valueStrings = filter.Split('>');

            foreach (var str in valueStrings)
            {
                Element.SendKeys(str);

            }
        }

        public void AssertEqualsTo(string value)
        {
            Assert.AreEqual(value, Element.Text);
        }

    }
}
