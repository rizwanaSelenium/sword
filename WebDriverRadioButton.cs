using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverRadioButton : WebDriverArmControl
    {
        public WebDriverRadioButton(IWebDriver driver, WebDriverWait waiter, string selector, bool isOldControl = false, bool shouldBeVisible = true)
            : base(driver, waiter, null)
        {
            ShouldBeVisible = shouldBeVisible;
            if (!isOldControl)
            {
                SetSelectorString("div.armcontrol#" + selector + " input[type='radio']");
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

        public void Select()
        {
            WaitUntilUiSpinnerIsNotDisplayed();
            WaitForElementToBeUsable();
            if(!Element.Selected)
                Element.Click();
        }
    }
}
