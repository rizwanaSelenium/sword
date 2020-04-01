using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverDropDown : WebDriverArmControl
    {
        private SelectElement _selectElement;

        private readonly bool _oldControl;
        private readonly string _selector;
        
        public WebDriverDropDown(IWebDriver driver, WebDriverWait waiter, string selector, bool isOldControl = false, bool shouldBeVisible = true)
            : base(driver, waiter, null)
        {
            _selector = selector;
            ShouldBeVisible = shouldBeVisible;
            if (!isOldControl)
            {
                SetSelectorString("div.armcontrol#" + selector);
                LabelElement = Driver.FindElement(By.CssSelector("div.armcontrol#" + selector + " div.lbl"));
                _oldControl = false;
            }
            else
            {
                SetSelectorString(selector);
                _oldControl = true;
            }

            RefreshSelectElement();
        }

        private void RefreshSelectElement()
        {
            _selectElement = _oldControl ? new SelectElement(Element) : new SelectElement(Driver.FindElement(By.CssSelector("div.armcontrol#" + _selector + " select")));
        }

        public void SetValue(string value, bool waitForWorkflow = false)
        {
            WaitForElementToBeUsable();
            try
            {
                _selectElement.SelectByText(value);
            }
            catch (NoSuchElementException nex)
            {
                throw new NoSuchElementException("Value: " + value + ", was not available in the dropdown list. WebDriverDropDown.cs - SetValue. Test name - " + TestContext.CurrentContext.Test.Name + ". " + nex);
            }

            if (!waitForWorkflow)
            { Waiter.Until(d => GetValue() == value);}
            WaitUntilUiSpinnerIsNotDisplayed();
        }

        public string GetValue()
        {
            WaitForElementToBeUsable();
            return _selectElement.SelectedOption.Text.Trim();
        }

        public void AssertEquals(string text)
        {
            Assert.AreEqual(text, _selectElement.SelectedOption.Text, string.Format("Expected dropdown value to be {0} but it was {1}.", text, _selectElement.SelectedOption.Text));
        }

        public void AssertDisabledDropDownEquals(string text)
        {
            Assert.AreEqual(text, _selectElement.SelectedOption.GetAttribute("title"), string.Format("Expected dropdown value to be {0} but it was {1}.", text, _selectElement.SelectedOption.GetAttribute("title")));
        }

        public void AssertOptionCountEquals(int count)
        {
            Assert.AreEqual(count, _selectElement.Options.Count, "Expected {0} options in the {1}, but there were {2}", count, CssSelectorString, _selectElement.Options.Count);
        }

        public void AssertContainsOptions(params string[] options)
        {
            var listOptions = _selectElement.Options.Select(o => o.Text).ToList();

            foreach (var option in options)
                Assert.True(listOptions.Contains(option), "Dropdown does not contain option '" + option + "'");
        }

        public void AssertDisabledDropDownContainsOptions(params string[] options)
        {
            var listOptions = _selectElement.Options.Select(o => o.GetAttribute("title")).ToList();

            foreach (var option in options)
                Assert.True(listOptions.Contains(option), "Dropdown does not contain option '" + option + "'");
        }

        public void AssertTextNotPresentInAnyOption(string optionText)
        {
            var listOptions = _selectElement.Options.Select(o => o.Text).ToArray();

            var textFound = listOptions.Any(listOption => listOption.Contains(optionText));

            if (textFound)
            {
                Assert.Fail("Expected Option {0} NOT to be present in any of the Drop Down List Options but it WAS", optionText);
            }
        }

        public IList<string> ListOfItems(params string[] options)
        {
            IList<string> items = _selectElement.Options.Select(o => o.Text).ToList();
            return items;

        }
        public override void AssertEnabled()
        {
            Assert.True(Element.Enabled);
        }

        public override void AssertDisabled()
        {
            if (!_oldControl)
            {
                var disabledElement = Driver.FindElement(By.CssSelector(CssSelectorString + " div.disabled-dropdown"));

                Assert.False(Element.Displayed);
                Assert.True(disabledElement.Displayed);
            }
            else
            {
                Assert.AreEqual("true", Element.GetAttribute("disabled"));
            }
        }

        public void AssertDisabledDropDownVisible()
        {
            var disabledElement = _oldControl ? Driver.FindElement(By.CssSelector(_selector + " div.disabled-dropdown")) : Driver.FindElement(By.CssSelector(CssSelectorString + " div.disabled-dropdown"));

            Assert.IsTrue(disabledElement.Displayed);
        }

        public new void AssertMandatory()
        {
            if (_oldControl)
            {
                base.AssertMandatory();
            }
            else
            {
                Assert.True(Driver.FindElement(By.CssSelector("div.armcontrol#" + _selector + " select")).GetAttribute("style").ToLower().Contains("background-color: #ffefd5") || Driver.FindElement(By.CssSelector("div.armcontrol#" + _selector + " select")).GetAttribute("style").ToLower().Contains("background-color: rgb(255, 239, 213)"), "Expected Element {0} {1} to be mandatory, but it was not", CssSelectorString, Driver.FindElement(By.CssSelector("div.armcontrol#" + _selector + " select")).GetAttribute("style"));
            }
            
        }

        public new void AssertNotMandatory()
        {
            if (_oldControl)
            {
                base.AssertNotMandatory();
            }
            else
            {
                Assert.False(Driver.FindElement(By.CssSelector("div.armcontrol#" + _selector + " select")).GetAttribute("style").ToLower().Contains("background-color: #ffefd5") || Driver.FindElement(By.CssSelector("div.armcontrol#" + _selector + " select")).GetAttribute("style").ToLower().Contains("background-color: rgb(255, 239, 213)"), "Expected Element {0} {1} to NOT be mandatory, but it WAS", CssSelectorString, Driver.FindElement(By.CssSelector("div.armcontrol#" + _selector + " select")).GetAttribute("style"));
            }
            
        }

        public new void AssertReadOnly()
        {
            if (_oldControl)
            {
                base.AssertReadOnly();
            }
            else
            {
                try
                {
                    string readOnlyAttribute = Driver.FindElement(By.CssSelector("div.armcontrol#" + _selector + " select")).GetAttribute("readOnly");
                    if (readOnlyAttribute == null)
                    {
                        Assert.Fail("Expected element '{0}' to be read only but it was not.", CssSelectorString);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Checking element with selector: " + CssSelectorString + " but got the error. " + e);
                }
                Assert.AreEqual("true", Driver.FindElement(By.CssSelector("div.armcontrol#" + _selector + " select")).GetAttribute("readOnly"), "Expected element '{0}' to be read only but it was not.", CssSelectorString);
            }
            
        }

        public new void AssertNotReadOnly()
        {
            if (_oldControl)
            {
                base.AssertNotReadOnly();
            }
            else
            {
                bool isReadOnly = true;
                try
                {
                    string readOnlyAttribute = Driver.FindElement(By.CssSelector("div.armcontrol#" + _selector + " select")).GetAttribute("readOnly");
                    if (readOnlyAttribute == null)
                    {
                        isReadOnly = false;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Checking element with selector: " + CssSelectorString + " but got the error. " + e);
                }
                if (isReadOnly)
                {
                    Assert.Fail("Expected element '{0}' NOT to be read only but it was.", CssSelectorString);
                }
            }
            
        }

        public new void AssertAccessKeyEquals(string key)
        {
            var actual = _selectElement.WrappedElement.GetAttribute("accessKey");

            Assert.AreEqual(key, actual, "Expected access key on element '{0}' to be '{1}' but it was '{2}'", CssSelectorString, key, actual);
        }

        public new void AssertTooltipEquals(string text)
        {
            var actual = _selectElement.WrappedElement.GetAttribute("title");
            Assert.AreEqual(text, actual, "Expected tooltip on element '{0}' to be '{1}' but was '{2}'.", CssSelectorString, text, actual);
        }
    }

    
}
