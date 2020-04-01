using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WdTableGridHeaderItem : WebDriverArmControl
    {
        public WdTableGridHeaderItem(IWebDriver driver, WebDriverWait waiter, string tableId, int column)
            : base(driver, waiter, null)
        {
            // Build the identifier to specify the intended cell (td)
            int tdCount = column - 1;
            var tdLocation = "td" + string.Concat(Enumerable.Repeat("+td", tdCount));

            // Build the appropriate CSS Selector String
            SetSelectorString("table#" + tableId + " tr " + tdLocation);
        }

        public string GetGridHeaderText()
        {
            return Element.Text;
        }

        public void AssertGridHeaderTextEquals(string gridHeaderText)
        {
            Assert.AreEqual(gridHeaderText, Element.Text);
        }
    }
}
