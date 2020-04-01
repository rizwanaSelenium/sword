using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverFormControl : WebDriverArmControl
    {
        public WebDriverFormControl(IWebDriver driver, WebDriverWait waiter, string selector) : base(driver, waiter, "form." + selector)
        {
  
        }

        public void AsserFieldset(string fieldset)
        {
           var field = Element.FindElements(By.CssSelector(" legend"));

            var value = field.Select(fld => fld.Text).ToList();
            Assert.True(value.Contains(fieldset));

        }
    }
}
