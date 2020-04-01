using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverTabControl : WebDriverArmControl
    {
        public WebDriverTabControl(IWebDriver driver, WebDriverWait waiter, string id) : base(driver, waiter, "table#" + id)
        {
        }

        public List<string> TableTextContentList()
        {
            IList<IWebElement> group = Driver.FindElements(By.CssSelector(CssSelectorString));
            List<string> names = group.Select(text => text.Text).ToList();
            return names;
        }
    }
}
