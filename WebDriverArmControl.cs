using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverArmControl
    {
        protected string XPathString;
        protected string CssSelectorString;
        protected string ElementId;
        protected IWebElement TagNameString;
        protected WebDriverWait Waiter;
        protected IWebDriver Driver;
        protected IWebElement Element
        {
            get
            {
                if (!string.IsNullOrEmpty(CssSelectorString))
                {
                    return Driver.FindElement(By.CssSelector(CssSelectorString));
                }

                if (TagNameString != null)
                {
                    return TagNameString.FindElement(By.Id(ElementId));
                }

                return Driver.FindElement(By.XPath(XPathString));
            }
         }

        protected IWebElement LabelElement;
        protected bool ShouldBeVisible = true;

        private string _id;
        public string Id
        {
            get
            {
                return _id ?? (_id = Element.GetAttribute("id"));
            }
        }

        public WebDriverArmControl(IWebDriver driver, WebDriverWait waiter, string selector)
        {
            Waiter = waiter;
            Driver = driver;

            if (!string.IsNullOrEmpty(selector))
            {
                SetSelectorString(selector);
            }
        }

        protected void SetSelectorString(string selector)
        {
            CssSelectorString = selector;
            WaitForElementToBeUsable();
        }

        public void WaitForTabPageToBeReady()
        {
            Waiter.Until(d => d.IsInitialDataLoadComplete());
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        protected void WaitUntilDesktopFooterIsDisplayed()
        {
            bool desktopFooterDisplayed = false;

            for (var i = 1; i < 30; i++)
            {
                Thread.Sleep(1000);

                var desktopFooters = Driver.FindElements(By.CssSelector("input[id^='AV_Grids_'][id$='Table_pagerText']")).Where(x => x.Displayed).ToList();

                if (desktopFooters.Any())
                {
                    desktopFooterDisplayed = true;
                    break;
                }
            }

            if (!desktopFooterDisplayed)
            {
                Console.WriteLine("Was waiting for the Desktop Footer Text To Be Displayed, but it was not displayed");
            }
        }

        protected void WaitUntilDesktopFooterIsEnabled()
        {
            bool desktopFooterEnabled = false;

            for (var i = 1; i < 30; i++)
            {
                Thread.Sleep(1000);

                var desktopFooters = Driver.FindElements(By.CssSelector("input[id^='AV_Grids_'][id$='Table_pagerText']")).Where(x => x.Enabled).ToList();

                if (desktopFooters.Any())
                {
                    desktopFooterEnabled = true;
                    break;
                }
            }

            if (!desktopFooterEnabled)
            {
                Console.WriteLine("Was waiting for the Desktop Footer Text To Be Enabled, but it was not enabled");
            }
        }

        public void WaitForElementToBeUsable()
        {
            if (ShouldBeVisible)
            {
                WaitForElementToAppear();
            }
            else
            {
                WaitForElementToExist();
            }
        }

        public void WaitForElementToAppear()
        {
            try
            {
                Waiter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(CssSelectorString)));
            }
            catch (Exception ex)
            {
                if (ex is WebDriverTimeoutException)
                {
                    throw new WebDriverTimeoutException("Timed out: couldn't find visible element " + CssSelectorString + " on page." + ex);
                }
                throw new Exception(ex.Message);
            }
        }

        public void WaitForElementToExist()
        {
            try
            {
                Waiter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.CssSelector(CssSelectorString)));
            }
            catch (Exception ex)
            {
                if (ex is WebDriverTimeoutException)
                {
                    throw new WebDriverTimeoutException("Timed out: couldn't find element " + CssSelectorString + " on page." + ex);
                }
                throw new Exception(ex.Message);
            }
        }

        public virtual void Click()
        {           
            var clickableElement =  Element ;
            try
            {
                WaitUntilUiSpinnerIsNotDisplayed();                
                Waiter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(clickableElement));
                clickableElement.Click();
            }
            catch (Exception ex)
            {
                if (ex is WebDriverTimeoutException)
                {
                    throw new WebDriverTimeoutException("Timed out waiting to click on element " + CssSelectorString + " on page. " + ex);
                }
                if (ex is ElementClickInterceptedException)
                {
                    throw new ElementClickInterceptedException("Found clickable element but something was blocking me clicking it. Element I wanted to click on - " + clickableElement + " - with CssString - " + CssSelectorString + " " + ex);
                }
                throw new Exception(ex.Message);
            }
        }

        public void AssertTextEquals(string text)
        {
            WaitForElementToAppear();
            Assert.AreEqual(text, Element.Text, string.Format("Expected text on element {0} to be {1} but it was {2}", CssSelectorString, text, Element.Text));
        }

        public void AssertTextContains(string text)
        {
            Assert.True(Element.Text.Contains(text), string.Format("Expected text on element {0} to contain {1} but it was {2}", CssSelectorString, text, Element.Text));
        }

        public virtual void AssertEnabled()
        {
            Assert.True(Driver.FindElement(By.CssSelector(CssSelectorString)).Enabled, "Expected element '{0}' to be enabled but it was not.", CssSelectorString);
        }

        public void AssertMandatory()
        {
            Assert.True(Element.GetAttribute("style").ToLower().Contains("background-color: #ffefd5") || Element.GetAttribute("style").ToLower().Contains("background-color: rgb(255, 239, 213)"), "Expected Element {0} {1} to be mandatory, but it was not", CssSelectorString, Element.GetAttribute("style"));
        }

        public void AssertNotMandatory()
        {
            Assert.False(Element.GetAttribute("style").ToLower().Contains("background-color: #ffefd5") || Element.GetAttribute("style").ToLower().Contains("background-color: rgb(255, 239, 213)"), "Expected Element {0} {1} to NOT be mandatory, but it WAS", CssSelectorString, Element.GetAttribute("style"));
        }

        public virtual void AssertDisabled()
        {
            Assert.False(Driver.FindElement(By.CssSelector(CssSelectorString)).Enabled, "Expected element '{0}' to be disabled but it was not.", CssSelectorString);
        }

        public void AssertVisible()
        {
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
            Assert.IsTrue(Element.Displayed, "Expected element '{0}' to be visible, but it was not.", CssSelectorString);
        }

        public void AssertAccessKeyEquals(string key)
        {
            var actual = Element.GetAttribute("accessKey");

            Assert.AreEqual(key, actual, "Expected access key on element '{0}' to be '{1}' but it was '{2}'", CssSelectorString, key, actual);
        }

        public void AssertLabelEquals(string text)
        {
            Assert.AreEqual(text, LabelElement.Text, "Expected label for element '{0}' to be '{1}' but was '{2}'.", CssSelectorString, text, LabelElement.Text);
        }

        public virtual void AssertTooltipEquals(string text)
        {
            var actual = Element.GetAttribute("title");
            Assert.AreEqual(text, actual, "Expected tooltip on element '{0}' to be '{1}' but was '{2}'.", CssSelectorString, text, actual);
        }

        public void AssertReadOnly()
        {
            try
            {
                string readOnlyAttribute = Element.GetAttribute("readOnly");
                if (readOnlyAttribute == null)
                {
                    Assert.Fail("Expected element '{0}' to be read only but it was not.", CssSelectorString);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Checking element with selector: " + CssSelectorString + " but got the error" + e);
            }
            Assert.AreEqual("true", Element.GetAttribute("readOnly"), "Expected element '{0}' to be read only but it was not.", CssSelectorString);
        }

        public void AssertNotReadOnly()
        {
            bool isReadOnly = true;
            try
            {
                string readOnlyAttribute = Element.GetAttribute("readOnly");
                if (readOnlyAttribute == null)
                {
                    isReadOnly = false;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Checking element with selector: " + CssSelectorString + " but got the error" + e);
            }
            if (isReadOnly)
            {
                Assert.Fail("Expected element '{0}' NOT to be read only but it was.", CssSelectorString);
            }
        }

        public IEnumerable<IWebElement> FindElements(By by)
        {
            return Element.FindElements(by);
        }

        public string GetAttribute(string attribute)
        {
            return Element.GetAttribute(attribute);
        }

        protected bool ElementIsPresent(By by)
        {
            try
            {
                Element.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void WaitUntilUiSpinnerIsNotDisplayed()
        {
            var errorCount = 0;
            int spinnerWait;
            const int spinnerMaxWait = 400;
            for (spinnerWait = 0 ; spinnerWait < spinnerMaxWait ; spinnerWait++)
            {
                try
                {
                    Thread.Sleep(1000);
                    var waiters = Driver.FindElements(By.CssSelector(".block-ui-spinner,.block-ui-message,.block-ui-wrapper,.blocked"));
                    if (waiters.Count == 0) break;
                    if (waiters.Count(e => e.Text == "No Configuration Selected") != 0) break; //The configuration blocker was being seen as a spinner blocker
                    if (waiters.Count(e => e.Displayed) != 0) continue;
                    break;
                }
                catch (Exception ex)
                {
                    errorCount = errorCount + 1;
                    if (errorCount >= 30)
                    {
                        throw new Exception("Errored " + errorCount + " times checking if block spinner was displayed test aborted. " + ex);
                    }
                }
            }
            if (errorCount != 0)
            {
                Console.WriteLine("** WebDriverArmControl.cs ** ** Error dectecing if the Block spinner was displayed or not. Times errored: " + errorCount);
            }
            if (spinnerWait == spinnerMaxWait)
            {
                throw new Exception("Block spinner was still displayed checking from WebDriverArmControl.cs");
            }
        }
    }
}
