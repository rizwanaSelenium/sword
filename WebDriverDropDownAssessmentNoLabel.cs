using System;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverDropDownAssessmentNoLabel : WebDriverArmControl
    {
        private readonly SelectElement _selectElement;

        public WebDriverDropDownAssessmentNoLabel(IWebDriver driver, WebDriverWait waiter, string selector)
            : base(driver, waiter, "div.armcontrol#" + selector)
        {
            SetSelectorString("div.armcontrol#" + selector + " select");
       
            
            _selectElement = new SelectElement(Element);
        }

        public void SetAssessmentDropDownValue(string value, bool isAssessmentPicker = false, bool isAdminDialog = false)
        {
            WaitForElementToAppear();
            WaitForTabPageToBeReady();

            Assert.True(_selectElement.Options.Select(o => o.Text).Contains(value));
            _selectElement.SelectByText(value);

            if (!isAssessmentPicker)
            {
                Waiter.Until(d => GetValue() == value);
            }
            else
            {
                Waiter.Until(d => GetValue() == "Undefined");
            }

            if (isAdminDialog)
            {
                try
                {
                    Driver.FindElement(By.CssSelector("div#UIPrompt"))
                        .FindElement(By.CssSelector("button[title='Yes']"))
                        .Click();
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("No Such Popup");
                }
            }
        }

        public void SetAssessmentDropDownUndefined()
        {
            if (_selectElement.Options.Count == 0 || _selectElement.SelectedOption.Text == "Undefined") return;
            Assert.True(_selectElement.Options.Select(o => o.Text).Contains("Undefined"));
            _selectElement.SelectByText("Undefined");
            WaitForTabPageToBeReady();
        }

        public void SetDefaultDropDownValue(string value)
        {
            WaitForElementToAppear();
            WaitForTabPageToBeReady();

            Assert.True(_selectElement.Options.Select(o => o.Text).Contains(value));
            _selectElement.SelectByText(value);
            WaitForTabPageToBeReady();

        }

        public string GetValue()
        {
            WaitForElementToAppear();
            return _selectElement.SelectedOption.Text;
        }

        public void AssertAssessmentDropDownValueEquals(string text)
        {
            WaitForElementToAppear();
            Assert.AreEqual(text, _selectElement.SelectedOption.Text);
        }

        public void AssertDisabledAssessmentDropDownValueEquals(string text)
        {
            var disabledElement = Driver.FindElement(By.CssSelector(CssSelectorString + " div.disabled-dropdown"));
            Assert.AreEqual(text, disabledElement.Text);
        }

        public void AssertAssessmentDropDownEnabled()
        {
            Assert.True(Element.Enabled);
        }

        public void AssertAssessmentDropDownDisabled()
        {
            var disabledElement = Driver.FindElement(By.CssSelector(CssSelectorString + " div.disabled-dropdown"));

            Assert.False(Element.Displayed);
            Assert.True(disabledElement.Displayed);
        }

        public void AssertAssessmentDropDownVisible()
        {
            Assert.IsTrue(Element.Displayed);
        }

        public void AssertDisabledAssessmentDropDownVisible()
        {
            var disabledElement = Driver.FindElement(By.CssSelector(CssSelectorString + " div.disabled-dropdown"));

            Assert.IsTrue(disabledElement.Displayed);
        }

        public void AssertAssessmentContainsOptions(params string[] options)
        {
            var listOptions = _selectElement.Options.Select(o => o.Text).ToList();

            foreach (var option in options)
                Assert.True(listOptions.Contains(option), "Dropdown does not contain option '" + option + "'");
        }

       
    }
}
