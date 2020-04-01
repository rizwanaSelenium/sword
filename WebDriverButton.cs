using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverButton : WebDriverArmControl
    {
        public WebDriverButton(IWebDriver driver, WebDriverWait waiter, string id, bool isNonStandardControl = false) : base(driver, waiter, null)
        {
            SetSelectorString(isNonStandardControl ? id : "button#" + id);
        }

        public override void Click()
        {
            try
            {
                Waiter.Until(x => Driver.FindElement(By.CssSelector(CssSelectorString)).Enabled);
                base.Click();
            }
            catch (TimeoutException tex)
            {
                throw new WebDriverTimeoutException("Expected element with selector: " + CssSelectorString + " to become enabled but it never did, waiter timed out. " + tex);
            }
        }

        public bool IsEnabled()
        {
            return Element.Enabled;
        }

        public bool IsDisplayed()
        {
            return Element.Displayed;
        }

    }
}
