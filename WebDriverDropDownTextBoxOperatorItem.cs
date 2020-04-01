using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverDropDownTextBoxOperatorItem : WebDriverArmControl
    {
        public WebDriverDropDownTextBoxOperatorItem(IWebDriver driver, WebDriverWait waiter, string selector) : base(driver, waiter, null)
        {
            SetSelectorString("tr.filter-criteria td." + selector);
      
        }

        public void SelectOperatorWithCriteria(string operatorOption, string filterCriteria)
        {
            const string elementSelector = "tr.filter-criteria td.operator div.dropDownTextBox";
            var elements = Driver.FindElements(By.CssSelector(elementSelector));

            if(!elements.Any())
                Assert.Fail("Could not find any items in the list '{0}'", elementSelector);

            var operatorDropDown = elements.Last();
            operatorDropDown.Click();

            const string operatorMenuSelector = "tr.filter-criteria td.operator div.b-m-mpanel[key='cmroot']";
            var operatorMenus = Driver.FindElements(By.CssSelector(operatorMenuSelector));

            if(!operatorMenus.Any())
                Assert.Fail("Could not find any items in the list '{0}'", operatorMenuSelector);

            var operatorMenu = operatorMenus.Last();
            var operatorItem = operatorMenu.FindElement(By.CssSelector("div[title='" + operatorOption + "']"));
            operatorItem.Click();

            var criteriaDropDownSelector = "div.armcontrol[id^='clone']";
            var criteriaDropDowns = Driver.FindElements(By.CssSelector(criteriaDropDownSelector));

            if(!criteriaDropDowns.Any())
                Assert.Fail("Could not find any items in the list '{0}'", criteriaDropDownSelector);

            var criteriaDropDown = criteriaDropDowns.Last();
            var cloneId = criteriaDropDown.GetAttribute("id");
            var filterCriteriaDropDown = new WebDriverDropDown(Driver, Waiter, "div.armcontrol#" + cloneId + " select", true);
            filterCriteriaDropDown.SetValue(filterCriteria);
        }
    }
}
