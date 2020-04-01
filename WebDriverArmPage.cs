using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PresentationModel.Controls.Angular;

namespace PresentationModel.Controls
{
    public class WebDriverArmPage : IDisposable
    {
        protected IWebDriver Driver;
        protected WebDriverWait Waiter;
        protected IList<WebDriverArmPage> Children;
        protected string PageName;
        private static string _thisWindowHandle;
        protected IJavaScriptExecutor Js;

        public WebDriverArmPage(IWebDriver driver, WebDriverWait waiter, string pageName)
        {
            Driver = driver;
            Waiter = waiter;
            PageName = pageName;
            Js = Driver as IJavaScriptExecutor;
            Children = new List<WebDriverArmPage>();
            FocusNewWindow();
        }

        private AngularModal _confirmationModal;
        public AngularModal ConfirmationModal
        {
            get {
                return _confirmationModal ?? (_confirmationModal = new AngularModal(Driver, Waiter));
            }
        }

        private AngularValidationErrorModal _validationErrorModal;
        public AngularValidationErrorModal ValidationErrorModal
        {
            get
            {
                return _validationErrorModal ?? (_validationErrorModal = new AngularValidationErrorModal(Driver, Waiter));
            }
        }

        private AngularEscalationModal _escalationModal;
        public AngularEscalationModal EscalationModal
        {
            get
            {
                return _escalationModal ?? (_escalationModal = new AngularEscalationModal(Driver, Waiter));
            }
        }

        private AngularStatusCommentsModal _statuscommentsModal;
        public AngularStatusCommentsModal StatusCommentsModal
        {
            get
            {
                return _statuscommentsModal ?? (_statuscommentsModal = new AngularStatusCommentsModal(Driver, Waiter));
            }
        }

        private AngularBlackFlagCommentsModal _blackflagcommentsModal;
        public AngularBlackFlagCommentsModal BlackFlagCommentsModal
        {
            get
            {
                return _blackflagcommentsModal ?? (_blackflagcommentsModal = new AngularBlackFlagCommentsModal(Driver, Waiter));
            }
        }

        private AngularScoringCommentsModal _angularscoringcommentsModal;
        public AngularScoringCommentsModal AngularScoringCommentsModal
        {
            get
            {
                return _angularscoringcommentsModal ?? (_angularscoringcommentsModal = new AngularScoringCommentsModal(Driver, Waiter));
            }
        }

        private AngularResponseAssessmentModal _angularResponseAssessmentModal;
        public AngularResponseAssessmentModal AngularResponseAssessmentModal
        {
            get
            {
                return _angularResponseAssessmentModal ?? (_angularResponseAssessmentModal = new AngularResponseAssessmentModal(Driver, Waiter));
            }
        }

        private AngularImpactHistoryModal _angularImpactHistoryModal;

        public AngularImpactHistoryModal AngularImpactHistoryModal
        {
            get
            {
                return _angularImpactHistoryModal ?? (_angularImpactHistoryModal = new AngularImpactHistoryModal(Driver, Waiter));
            }
        }

        private WebDriverButton _swordLogo;
        public WebDriverButton SwordLogo
        {
            get { return _swordLogo ?? (_swordLogo = new WebDriverButton(Driver, Waiter, "navigator-header")); }
        }

        public void Refresh()
        {
            Driver.Navigate().Refresh();
            WaitUntilPageIsReady();
        }

        protected void WaitUntilPageIsReady()
        {
            Waiter.Until(d => d.HasWindowWithName(PageName));
            
            // Some of these scripts are incredibly prone to failure - throwing 'Javascript errors' with no stack trace.
            // We're left with little option but to try and run the script, then wait a period of time before continuing.
            // Ideally of course we would handle the exception and fix the root cause, but IE gives us absolutely nothing to work with here.
            try
            {
                Waiter.Until(d => Driver.ExecuteScript("return !!window.document ? window.document.readyState : false").Equals("complete"));
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Automation Faield in WaitUntilPageIsReady - WebDriverArmPage.cs -- Reporting Only -- Test continues.");
            }

            WaitUntilUiSpinnerIsNotDisplayed();
        }

        protected T OpenChildDialog<T>() where T : WebDriverArmPage
        {
            var child = (T) Activator.CreateInstance(typeof(T), Driver, Waiter);
            Children.Add(child);
            child.FocusWindow();
            child.WaitUntilUiSpinnerIsNotDisplayed();
            return child;
        }

        public void AssertUrlEndsWith(string text)
        {
            Assert.True(Driver.Url.EndsWith(text), "Expected URL to end with: " + text + " but was actually: " + Driver.Url);
        }

        public void AssertTitleEquals(string expected)
        {
            Assert.AreEqual(expected, Driver.Title);
        }

        [Obsolete("Use FocusNewWindow",false)]
        public void FocusWindow() 
        {
            if (Driver.WindowHandles.Contains(_thisWindowHandle))
                Driver.SwitchTo().Window(_thisWindowHandle);
            WaitUntilPageIsReady();
        }

        public void FocusNewWindow()
        {
            if (_thisWindowHandle != Driver.WindowHandles.Last())
            {
                Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                _thisWindowHandle = Driver.WindowHandles.Last();
            }
            else
            {
                _thisWindowHandle = Driver.WindowHandles.Last();
            }
        }

        public void AssertAlertShown()
        {
            try
            {
                Waiter.Until(d => d.HasAlertWindow());
                Driver.SwitchTo().Alert().Dismiss();
            }
            catch (WebDriverTimeoutException tex)
            {
                throw new WebDriverTimeoutException("Expected alert for unsaved changes. " + tex);
            }
        }

        public void RespondToUiPrompt(string buttonText)
        {
            FocusWindow();
            Waiter.Until(d => d.FindElement(By.CssSelector("div#UIPrompt")));

            var prompt = Driver.FindElement(By.CssSelector("div#UIPrompt"));
            prompt.FindElement(By.CssSelector("button[title='" + buttonText + "']")).Click();

            bool uiPromptDisplayed = true;

            for (var i = 1; i < 30; i++)
            {
                Thread.Sleep(1000);
                var uiPrompts = Driver.FindElements(By.CssSelector("div#UIPrompt")).Where(x => x.Displayed).ToList();

                if (uiPrompts.Count == 0)
                {
                    uiPromptDisplayed = false;
                    break;
                }
            }

            if (uiPromptDisplayed)
            {
                Assert.Fail("Was waiting for the UI Prompt to stop being displayed after responding with {0}, but it was still displayed", buttonText);
            }
        }

        public void AssertUiPromptContainsText(string expectedtext)
        {
            FocusWindow();
            bool promptPresent = false;

            for (var i = 0; i < 5; i++)
            {
                Thread.Sleep(500);
                var expectedPrompts = Driver.FindElements(By.CssSelector("div#UIPrompt"));
                if (expectedPrompts.Count == 1)
                {
                    promptPresent = true;
                    break;
                }
            }

            if (!promptPresent)
            {
                Assert.Fail("Was waiting for a Prompt to be displayed on the Desktop, but it was NOT displayed");
            }

            var promptElements = Driver.FindElements(By.CssSelector("div#UIPrompt td"));
            bool promptTextFound = false;

            foreach (var promptElement in promptElements)
            {
                if (promptElement.Text.Contains(expectedtext))
                {
                    promptTextFound = true;
                    break;
                }
            }

            if (!promptTextFound)
            {
                // Prompt Text was not found, so attempt to close the Prompt and Fail
                RespondToUiPrompt("No");
                Assert.Fail("Expected the Desktop Prompt to contain {0}, but it DID not contain this Text", expectedtext);
            }
        }

        public void AcceptAlert()
        {
            for (var i = 0; i < 5; i++)
            {
                Thread.Sleep(1000);
                if (Driver.HasAlertWindow())
                {
                    var alert = Driver.SwitchTo().Alert();
                    alert.Accept();
                    break;
                }
                Console.WriteLine("Alert Not Found for " + (i + 1) + " time");
            }
        }

       public void RespondToModalDialog(string buttonText)
       {
           FocusWindow();
           Waiter.Until(d => d.FindElement(By.CssSelector("div.modal-content")));
           var prompt = Driver.FindElement(By.CssSelector("div.modal-content"));

           var buttons = prompt.FindElements(By.CssSelector("button.btn-arm"));
           foreach (var button in buttons)
           {
               if (button.Text == buttonText)
               {
                   button.Click();
                   break;
               }
           }
           Waiter.Until(d => !d.IsAjaxRequestInProgress());
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
                Console.WriteLine("** WebDriverArmPage.cs ** ** Error dectecing if the Block spinner was displayed or not. Times errored: " + errorCount);
            }
            if (spinnerWait == spinnerMaxWait)
            {
                throw new Exception("Block spinner was still displayed checking from WebDriverArmPage.cs");
            }
        }

        public bool IsMessageModalPopUpDisplayed()
        {
            try
            {
                bool displayed = Driver.FindElement(By.CssSelector("#modalMessageBox")).Displayed;
                return displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsEscalationModalDisplayed()
        {
            try
            {
                bool displayed = Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed;
                return displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void ScrollToElement(object element)
        {
            Js.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        public virtual void Close()
        {
            Dispose();
        }

        private void ReleaseUnmanagedResources()
        {
            FocusWindow();
            Driver.Close();

            new WebDriverWait(Driver, TimeSpan.FromSeconds(1)).Until(d => d.HasAlertWindow());
            Driver.SwitchTo().Alert().Accept();
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                Driver.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
