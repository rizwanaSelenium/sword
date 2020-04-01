using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WdAlternatingGrid : WebDriverArmControl
    {
        public WdAlternatingGrid(IWebDriver driver, WebDriverWait waiter, string selector) : base(driver, waiter, selector)
        {

        }

        public void AddParameter(string parameterName)
        {
            var parameters = Element.FindElements(By.CssSelector("div[ng-repeat]"));

            bool parameterFound = false;

            foreach (var parameter in parameters)
            {
                if (parameter.FindElement(By.CssSelector("div.ng-binding")).Text == parameterName)
                {
                    parameterFound = true;
                    var addParameterToMessagePointer = parameter.FindElement(By.CssSelector("div.parameter-add-button"));

                    Driver.ExecuteScript("arguments[0].click();", addParameterToMessagePointer);
                    break;
                }
            }

            if (!parameterFound)
            {
                Assert.Fail("Parameter: " + parameterName + " Not Found In Parameter List");
            }
        }
    }
}
