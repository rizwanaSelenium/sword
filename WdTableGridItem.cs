using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WdTableGridItem : WebDriverArmControl
    {
        public WdTableGridItem(IWebDriver driver, WebDriverWait waiter, string tableId, int row, int column)
            : base(driver, waiter, null)
        {
            // Build the identifier to specify the intended row (tr)
            int trCount = row;
            var trLocation = "tr" + string.Concat(Enumerable.Repeat("+tr", trCount));

            // Build the identifier to specify the intended cell (td)
            int tdCount = column - 1;
            var tdLocation = "td" + string.Concat(Enumerable.Repeat("+td", tdCount));

            // Build the appropriate CSS Selector String
            SetSelectorString("table#" + tableId + " " + trLocation + " " + tdLocation);
        }

        public string GetGridItemText()
        {
            return Element.Text;
        }

        public void AssertGridItemTextEquals(string gridItemText)
        {
            Assert.AreEqual(gridItemText, Element.Text);
        }
    }
}
