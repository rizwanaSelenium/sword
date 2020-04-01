using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class FileSelector : WebDriverArmControl
    {
        public FileSelector(IWebDriver driver, WebDriverWait waiter, string selector) : base(driver, waiter, null)
        {
            SetSelectorString("div.armcontrol#" + selector);
            var fileSelector = "div.armcontrol input[id='" + selector + "_tb'][type='file']";
            SetSelectorString(fileSelector);
            LabelElement = Driver.FindElement(By.CssSelector("div.armcontrol#" + selector + " div.lbl"));
        }
    }
}
