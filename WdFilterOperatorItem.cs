using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WdFilterOperatorItem : WebDriverArmControl
    {
        public WdFilterOperatorItem(IWebDriver driver, WebDriverWait waiter, string selector) : base(driver, waiter, null)
        {
            SetSelectorString("tr.filter-criteria td." + selector);
        }

        public void SelectFilterOperatorWithCriteria(string operatorToSelect, string operatorCriteriaToSelect)
        {
            var operators = Driver.FindElements(By.CssSelector("tr.filter-criteria td.operator div.dropDownTextBox"));
            var operatorDropDown = operators.Last();
            operatorDropDown.Click();

            var operatorMenus = Driver.FindElements(By.CssSelector("tr.filter-criteria td.operator div.b-m-mpanel[key='cmroot']"));
            var operatorMenu = operatorMenus.Last();
            
            bool operatorFound = false;

            var operatorOptions = operatorMenu.FindElements(By.CssSelector("div.b-m-item"));

            foreach (var operatorOption in operatorOptions)
            {
                if (operatorOption.FindElement(By.CssSelector("span")).Text.Equals(operatorToSelect))
                {
                    operatorFound = true;
                    operatorOption.FindElement(By.CssSelector("div.b-m-ibody")).Click();
                    break;
                }
            }

            if (!operatorFound)
            {
                Assert.Fail("Operator: " + operatorToSelect + " not found");
            }

            var operatorCriteriaFields = Driver.FindElements(By.CssSelector("div.armcontrol[id^='clone']"));
            var operatorCriteriaField = operatorCriteriaFields.Last();
            var cloneId = operatorCriteriaField.GetAttribute("id");
            var filterOperatorCriteriaTextField = new WebDriverTextField(Driver, Waiter, "div.armcontrol[id='" + cloneId + "'] input[type='text']", true);
            filterOperatorCriteriaTextField.SetValue(operatorCriteriaToSelect);
        }

        public void SelectFilterOperatorWithOwnerCriteria(string operatorToSelect, string ownerPrefixToEnter, string operatorOwnerCriteriaToSelect)
        {
            var operators = Driver.FindElements(By.CssSelector("tr.filter-criteria td.operator div.dropDownTextBox"));
            var operatorDropDown = operators.Last();
            operatorDropDown.Click();

            var operatorMenus = Driver.FindElements(By.CssSelector("tr.filter-criteria td.operator div.b-m-mpanel[key='cmroot']"));
            var operatorMenu = operatorMenus.Last();

            bool operatorFound = false;

            var operatorOptions = operatorMenu.FindElements(By.CssSelector("div.b-m-item"));

            foreach (var operatorOption in operatorOptions)
            {
                if (operatorOption.FindElement(By.CssSelector("span")).Text.Equals(operatorToSelect))
                {
                    operatorFound = true;
                    operatorOption.FindElement(By.CssSelector("div.b-m-ibody")).Click();
                    break;
                }
            }

            if (!operatorFound)
            {
                Assert.Fail("Operator: " + operatorToSelect + " not found");
            }

            var operatorCriteriaFields = Driver.FindElements(By.CssSelector("div.armcontrol[id^='clone']"));
            var operatorCriteriaField = operatorCriteriaFields.Last();
            var cloneId = operatorCriteriaField.GetAttribute("id");
            var filterOperatorCriteriaTextField = new WebDriverTextField(Driver, Waiter, "div.armcontrol[id='" + cloneId + "'] input[type='text']", true);
            filterOperatorCriteriaTextField.SetValue(operatorOwnerCriteriaToSelect);
        }
    }
}
