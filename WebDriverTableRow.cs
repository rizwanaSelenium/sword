using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverTableRow : WebDriverArmControl
    {
        public WebDriverTableRow(IWebDriver driver, WebDriverWait waiter, string index, string tableSelectorString = "") : base(driver, waiter, tableSelectorString + "tr.grid-row[index='" + index + "']")
        {
        }

        public void CheckRow()
        {
            var checkbox = Element.FindElement(By.CssSelector("td input.grid-checkbox"));

            if(!checkbox.Selected)
                checkbox.Click();
        }

        public void UncheckRow()
        {
            var checkbox = Element.FindElement(By.CssSelector("td input.grid-checkbox"));

            if (checkbox.Selected)
                checkbox.Click();
        }

        public void ClickRow()
        {
            Element.Click();
        }

        public WebDriverTableCell GetCell(int columnNumber)
        {
            var cellId = Element.FindElements(By.CssSelector("td.grid-cell"))[columnNumber].GetAttribute("id");
            return new WebDriverTableCell(Driver, Waiter, cellId);
        }
    }
}
