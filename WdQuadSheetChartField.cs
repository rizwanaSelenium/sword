using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WdQuadSheetChartField : WebDriverArmControl
    {
        public WdQuadSheetChartField(IWebDriver driver, WebDriverWait waiter, string identifier, string spanId)
            : base(driver, waiter, "div#" + identifier + " span#" + spanId)
        {

        }

        public string GetText()
        {
            WaitForElementToAppear();
            return Element.Text;
        }
    }
}
