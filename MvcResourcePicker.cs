using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class MvcResourcePicker : WebDriverArmControl
    {
        public MvcResourcePicker(IWebDriver driver, WebDriverWait waiter, string selector)
            : base(driver, waiter, "arm-resource-picker input#" + selector)
        {
            var elementParent = Element.FindElement(By.XPath(".." + "/" + ".." + "/" + ".."));
            LabelElement = elementParent.FindElement(By.CssSelector("label"));
        }

        public void EnterFullValueAndPressEnter(string value)
        {
            Element.Click();
            Element.Clear();
            Element.SendKeys(value);

            Waiter.Until(d => Element.FindElement(By.XPath("..")).FindElement(By.CssSelector("li[id^='typeahead']")).Displayed);

            Element.SendKeys(Keys.Enter);
        }

        /// <summary>
        /// Gets the real value of the field which might not be in the attribute i.e. set by angular by element.value = ...
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            try
            {
                return Driver.ExecuteJavaScript<string>("return jQuery(arguments[0]).val()", CssSelectorString);
            }
            catch
            {
                return string.Empty;
            }
        }

        public void AssertEquals(string expectedValue)
        {
            WaitForElementToAppear();
            Assert.AreEqual(expectedValue, GetValue());
        }

        public void DeleteValue()
        {
            Element.Clear();
            Waiter.Until(d => Element.Text.Length.Equals(0));
        }
    }
}
