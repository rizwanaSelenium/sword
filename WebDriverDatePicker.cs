using System;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverDatePicker : WebDriverArmControl
    {
        private SelectElement MonthSelector
        {
            get
            {
                return new SelectElement(Driver.FindElement(By.CssSelector("div.ui-datepicker div.ui-datepicker-header select.ui-datepicker-month")));
            }
        }

        private SelectElement YearSelector
        {
            get
            {
                return new SelectElement(Driver.FindElement(By.CssSelector("div.ui-datepicker div.ui-datepicker-header select.ui-datepicker-year")));
            }
        }

        private WebDriverButton PickButton { get; set; }

        public WebDriverDatePicker(IWebDriver driver, WebDriverWait waiter, string id, bool isOldControl = false)
            : base(driver, waiter, null)
        {
            if (!isOldControl)
            {
                SetSelectorString("div#" + id + " input.hasDatepicker");
                LabelElement = driver.FindElement(By.CssSelector("div#" + id + " div.lbl"));
                // If the Element is Disabled, then there SHOULD NOT be a Pick Button
                try
                {
                    if (Element.GetAttribute("disabled").Equals("disabled"))
                    {
                        // If the Element is disabled then there SHOULD NOT be a Pick Button
                    }
                }
                catch (Exception)
                {
                    // Disabled element is not set, so Element is NOT disabled, so it SHOULD have a Pick Button
                    PickButton = new WebDriverButton(driver, waiter, Driver.FindElement(By.CssSelector("div#" + id + " button")).GetAttribute("id"));
                }
            }
            else
            {
                SetSelectorString(id + " input");
                // If the Element is Disabled, then there SHOULD NOT be a Pick Button
                try
                {
                    if (Element.GetAttribute("disabled").Equals("disabled"))
                    {
                        // If the Element is disabled then there SHOULD NOT be a Pick Button
                    }
                }
                catch (Exception)
                {
                    // Disabled element is not set, so Element is NOT disabled, so it SHOULD have a Pick Button
                    PickButton = new WebDriverButton(driver, waiter, Driver.FindElement(By.CssSelector(id + " button")).GetAttribute("id")); 
                }
            }
        }

        public void PickDate(DateTime date)
        {
            PickButton.Click();

            MonthSelector.SelectByValue((date.Month - 1).ToString());
            YearSelector.SelectByValue(date.Year.ToString());

            Waiter.Until(d => d.FindElement(By.CssSelector("table.ui-datepicker-calendar td a")).Displayed);

            var days = Driver.FindElements(By.CssSelector("table.ui-datepicker-calendar td a"));
            days.Single(d => d.Text == date.Day.ToString()).Click();

            Waiter.Until(d => !d.FindElement(By.CssSelector("table.ui-datepicker-calendar td a")).Displayed);
        }

        public void AssertValue(string date)
        {
            if (!String.IsNullOrEmpty(date))
            {
                var comparisonDate = DateTime.Parse(date).Date;
                var dateInElement = DateTime.Parse(Element.GetAttribute("value")).Date;
                Assert.AreEqual(comparisonDate, dateInElement, string.Format("Expected date of element {0} to be {1} but it was {2}", CssSelectorString, comparisonDate, dateInElement));
            }
            else
            {
                Assert.AreEqual(date, Element.GetAttribute("value"), string.Format("Expected date of element {0} to be {1} but it was {2}", CssSelectorString, date, Element.GetAttribute("value")));
            }
        }

        public void SetToday()
        {
            PickDate(DateTime.Now);
        }

        public void SetTomorrow()
        {
            PickDate(DateTime.Now.AddDays(1));
        }

        public void SetOneWeekFromToday()
        {
            PickDate(DateTime.Now.AddDays(7));
        }

        public void AssertToday()
        {
            var dateInElement = DateTime.Parse(Element.GetAttribute("value"));
            Assert.AreEqual(DateTime.Today.Date, dateInElement.Date , string.Format("Expected date for element {0} to be Today but it was {1}", CssSelectorString, dateInElement.Date));
        }

        public void AssertTomorrow()
        {
            var dateInElement = DateTime.Parse(Element.GetAttribute("value"));
            Assert.AreEqual(DateTime.Today.AddDays(1).Date, dateInElement, string.Format("Expected date for element {0} to be Tomorrow but it was {1}", CssSelectorString, dateInElement.Date));
        }

        public void AssertOneWeekFromToday()
        {
            AssertDateIsNumberOfDaysFromToday(7);
        }

        public void AssertDateIsNumberOfDaysFromToday(int numberOfDaysFromToday)
        {
            var dateInElement = DateTime.Parse(Element.GetAttribute("value"));
            Assert.AreEqual(DateTime.Today.AddDays(numberOfDaysFromToday).Date, dateInElement, string.Format("Expected date for element {0} to be {1} but it was {2}", CssSelectorString, DateTime.Today.AddDays(numberOfDaysFromToday).Date, dateInElement.Date));
        }

        public new void AssertMandatory()
        {
            Assert.True(Element.GetAttribute("style").ToLower().Contains("background-color: #ffefd5") || Element.GetAttribute("style").ToLower().Contains("background-color: rgb(255, 239, 213)"));
        }

        public void Clear()
        {
            PickButton.Click();
            Driver.FindElement(By.CssSelector("div.ui-datepicker-buttonpane button.ui-datepicker-close")).Click();
        }
    }
}
