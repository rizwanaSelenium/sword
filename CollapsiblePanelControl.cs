using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class CollapsiblePanelControl : WebDriverArmControl
    {
        public CollapsiblePanelControl(IWebDriver driver, WebDriverWait waiter, string sectionId)
            : base(driver, waiter, null)
        {
            SetSelectorString("div#" + sectionId + " a#" + sectionId);            
        }

        public void Expand()
        {
            WaitForElementToAppear();
            if (Element.GetAttribute("aria-expanded") == "true") return;
            Element.FindElement(By.CssSelector("span#expand-icon")).Click();
            try
            {
                Waiter.Until(d => Element.GetAttribute("aria-expanded") == "true");
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new WebDriverTimeoutException("Timed-out while waiting for the collapsible panel section to expand. " + ex);
            }
        }

        public void Collapse()
        {
            WaitForElementToAppear();
            if (Element.GetAttribute("aria-expanded") == "false") return;
            Element.FindElement(By.CssSelector("span#collapse-icon")).Click();
            try
            {
                Waiter.Until(d => Element.GetAttribute("aria-expanded") == "false");
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new WebDriverTimeoutException("Timed-out while waiting for the collapsible panel section to collapse. " + ex);
            }
        }
    }
}
