using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverTickBoxControl : WebDriverArmControl
    {
        public WebDriverTickBoxControl(IWebDriver driver, WebDriverWait waiter, string selector, bool isOldControl = false, bool shouldBeVisible = true) : base(driver, waiter, null)
        {
            ShouldBeVisible = shouldBeVisible;
            if (!isOldControl)
            {
                SetSelectorString("div.armcontrol#" + selector + " input[type='checkbox']");
                LabelElement = Driver.FindElement(By.CssSelector("div.armcontrol#" + selector + " div.lbl"));
            }
            else
            {
                SetSelectorString(selector);
            }
        }

        public void AssertChecked()
        {
            Assert.True(Element.Selected);
        }

        public void AssertUnchecked()
        {
            Assert.False(Element.Selected);
        }



        public override void Click()
        {
            WaitForElementToBeUsable();
            Element.Click();
        }

        public void Check()
        {
            WaitForElementToBeUsable();

            if(!Element.Selected)
                Element.Click();
        }

        public void Uncheck()
        {
            WaitForElementToBeUsable();
            
            if (Element.Selected)
                Element.Click();
        }

        public bool GetStatus()
        {
            return (Element.Selected);
        }

        public void AssertNotDisplayed()
        {
           Assert.False(Element.Displayed);
        }

        public bool DisplayedStatus()
        {
            return (Element.Displayed);
        }
    }
}
