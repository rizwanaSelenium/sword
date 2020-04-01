using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverTextField : WebDriverArmControl
    {
        public WebDriverTextField(IWebDriver driver, WebDriverWait waiter, string selector, bool isOldControl = false)
            : base(driver, waiter, null)
        {
            if (!isOldControl)
            {
                SetSelectorString("div.armcontrol#" + selector + " input#" + selector + "_tb");
                LabelElement = Driver.FindElement(By.CssSelector("div.armcontrol#" + selector + " div.lbl"));
            }
            else
            {
                SetSelectorString(selector);
            }
        }

        public virtual void SetValue(string value)
        {
            Element.Clear();
            Element.SendKeys(value);
        }

        public void Enter()
        {
            Element.SendKeys(Keys.Return);
        }

        public void DeleteValue()
        {
            Element.Click();
            Element.SendKeys(Keys.End);
            var length = GetValue().Length;
            int i = 0;
            while (i < length)
            {
                Element.SendKeys(Keys.Backspace);
                i++;
            }
        }

        public void AssertNotEquals(string diferentValue)
        {
            Assert.AreNotEqual(diferentValue, GetValue());
        }

        public void AssertEquals(string text)
        {
            Assert.AreEqual(text, GetValue());
        }

        public void AssertLabelTextEquals(string text)
        {
            Assert.AreEqual(text, LabelElement.Text);
        }

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

        public void AssertTextFieldMandatory()
        {
            Assert.True(Element.GetAttribute("style").ToLower().Contains("background-color: #ffefd5") || Element.GetAttribute("style").ToLower().Contains("background-color: rgb(255, 239, 213)"));
        }

        public void AppendValue(string value)
        {
            WaitForElementToAppear();
            Element.SendKeys(Keys.End);
            Element.SendKeys(value);
        }

        public string GetText()
        {
            WaitForElementToAppear();
            var getelementText = Element.Text;
            return getelementText;

        }

        public void AsertTextEquals(string text)
        {
            WaitForElementToAppear();
            Assert.AreEqual(text,GetText());
        }
    }
}
