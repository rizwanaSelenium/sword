using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverTextAreaControl : WebDriverArmControl
    {
        public WebDriverTextAreaControl(IWebDriver driver, WebDriverWait waiter, string selector,
            bool isNewControl = false)
            : base(driver, waiter, null)
        {
            if (isNewControl)
            {
                SetSelectorString(selector);
            }
            else
            {
                SetSelectorString("div.armcontrol#" + selector + " textarea");
                LabelElement = Driver.FindElement(By.CssSelector("div.armcontrol#" + selector + " div.lbl"));
            }
           
        }

        public void SetValue(string value)
        {
            WaitForElementToAppear();
            Element.Clear();
            Element.SendKeys(value);
        }

        public void AppendValue(string value)
        {
            WaitForElementToAppear();
            Element.SendKeys(Keys.End);
            Element.SendKeys(value);
        }

        public void Clear()
        {
            WaitForElementToAppear();
            Element.Clear();
        }
        public string GetValue()
        {
            WaitForElementToAppear();
            return Element.Text;
        }

        public void AssertTextAreaMandatory()
        {
            Assert.True(Element.GetAttribute("style").ToLower().Contains("background-color: #ffefd5") || Element.GetAttribute("style").ToLower().Contains("background-color: rgb(255, 239, 213)"));
        }

        public void AssertTextAreaMandatoryFieldsHighlighted()
        {
            Assert.True(Element.GetAttribute("style").Contains("border-color: red"));
        }

        public void AssertAssessmentTextAreaTextIs(string value)
        {
            WaitForElementToAppear();
            Assert.AreEqual(value, Element.Text);
        }

        public bool IsDisplayed()
        {
            return Element.Displayed;
        }
    }
}
