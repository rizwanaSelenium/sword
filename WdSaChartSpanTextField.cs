using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WdSaChartSpanTextField : WebDriverArmControl
    {
        public WdSaChartSpanTextField(IWebDriver driver, WebDriverWait waiter, string spanId) : base(driver, waiter, "span#" + spanId)
        {

        }

        public string GetSpanText()
        {
            WaitForElementToAppear();

            return Element.Text;
        }

        public void AssertSpanTextEquals(string expectedValue)
        {
            WaitForElementToAppear();

            Assert.AreEqual(expectedValue, Element.Text);
        }
    }
}
