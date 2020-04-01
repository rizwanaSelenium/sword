using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverChartControl : WebDriverArmControl
    {

        public WebDriverChartControl(IWebDriver driver, WebDriverWait waiter, string selector, bool isOldControl = false)
            : base(driver, waiter, null)
        {
            if (!isOldControl)
            {
                SetSelectorString("div#chartDiv #" + selector);
            }
            else
            {
                SetSelectorString(selector);
            }
        }

        public void AssertChartPresent()
        {
            var chartDisp = Element.Displayed;
            if (!chartDisp)
            {
                Assert.Fail("Chart is not present");
            }
        }

        public void AssertShowPidforQuadSheet()
        {
            var visiblecells = Element.FindElement(By.CssSelector("img"));
            if (!visiblecells.Displayed)
            {
                Assert.Fail("Quad sheet chart not showing PID");
            }
           
        }

        public void AssertShowcoloursforQuadSheet()
        {
            var a = Element.FindElement(By.CssSelector("td[colourRef='#0000FF']")).GetAttribute("bgcolor");
            if (String.IsNullOrEmpty(a))
            {
                Assert.Fail("Quad sheet chart not showing Colours");
            }

            else
            {
                Assert.True(a == "#0000ff", "Quad sheet is showing colours");
            }
            
        }
    }
}
