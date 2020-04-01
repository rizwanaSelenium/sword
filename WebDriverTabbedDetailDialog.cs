using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PresentationModel.Model.Help;

namespace PresentationModel.Controls
{
    public abstract class WebDriverTabbedDetailDialog : WebDriverArmPage
    {
        private WebDriverDetailTabControl _tabControl;
        public WebDriverDetailTabControl TabControl
        {
            get { return _tabControl ?? (_tabControl = new WebDriverDetailTabControl(Driver, Waiter, "DV_DTC")); }
        }

        protected Dictionary<string, string> HelpPages;

        protected WebDriverTabbedDetailDialog(IWebDriver driver, WebDriverWait waiter, string pageName) : base(driver, waiter, pageName)
        {
            
        }

        public string ActiveTabName()
        {
            return TabControl.ActiveTabName();
        }

        public void SwitchToTab(string tabName)
        {
            FocusWindow();
            TabControl.SwitchTo(tabName);
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                if (ActiveTabName() != tabName)
                {
                    TabControl.SwitchTo(tabName);
                    Console.WriteLine("SwitchTab failed " + (i + 1) + " times");
                }
                else
                {
                    break;
                }
            }
            WaitForTabPageToBeReady(tabName);
        }

        public virtual void AssertHelpPageCorrect()
        {
            var helpPage = HelpPages[ActiveTabName()];
            var page = new WebDriverHelpPage(Driver, Waiter, helpPage);
            page.AssertUrlEndsWith(helpPage);
            page.Close();
        }
        
        public void WaitForTabPageToBeReady(string tabName)
        {
            bool tabNameNotChanged = true;
            for (int i = 0; i < 15; i++)
            {
                Thread.Sleep(1000);
                if (ActiveTabName() == tabName)
                {
                    tabNameNotChanged = false;
                    break;
                }
                Console.WriteLine("TabName has not changed when checked " + (i+1) + " times");
            }
            if (tabNameNotChanged)
            {
                Assert.Fail("Tab Name Did Not Change is Currently: " + ActiveTabName() + ", Expected: " + tabName);
            }
            DialogueLoadingBlockersIsNotDisplayed();
        }

        public Boolean NoAccessToTab(string tabName)
        {
            FocusWindow();
   
            if (TabControl.AssertTabDisabled(tabName))
            {
                FocusWindow();
                TabControl.SwitchTo(tabName);
                String currentTabName = ActiveTabName();
                if (currentTabName != tabName)
                    return true;
                else
                    return false;
            }
            return false;
        }

        private void DialogueLoadingBlockersIsNotDisplayed()
        {
            bool dialogueLoadingBlockersDisplayed = true;
            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                var dialogueLoadingBlockers = Driver.FindElements(By.XPath("//div[@class='blocked']")).Where(x => x.Displayed).ToList();
                if (dialogueLoadingBlockers.Count == 0)
                {
                    dialogueLoadingBlockersDisplayed = false;
                    break;
                }

            }
            if (dialogueLoadingBlockersDisplayed)
            {
                Assert.Fail("Was waiting for the Tabbed Dialogue Blocker to stop being displayed, but it was still displayed on the Risk Dialogue");
            }
        }
    }
}
