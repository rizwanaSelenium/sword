using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    class WedDriverBootstrapTickBoxControl : WebDriverArmControl
    {
        public WedDriverBootstrapTickBoxControl(IWebDriver driver, WebDriverWait waiter, string selector, bool shouldBeVisible = false) : base(driver, waiter, null)
        {
                ShouldBeVisible = shouldBeVisible;
                SetSelectorString(selector + " input[type='checkbox']");
                LabelElement = Driver.FindElement(By.CssSelector(selector + " div.checkbox label"));
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
            LabelElement.Click();
        }

        public void Check()
        {
            WaitForElementToBeUsable();

            if (!Element.Selected)
                LabelElement.Click();
        }

        public void Uncheck()
        {
            WaitForElementToBeUsable();

            if (Element.Selected)
                LabelElement.Click();
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
