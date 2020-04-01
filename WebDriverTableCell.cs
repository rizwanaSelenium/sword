using System;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverTableCell : WebDriverArmControl
    {
        private readonly string _selector;

        public WebDriverTableCell(IWebDriver driver, WebDriverWait waiter, string selector) : base(driver, waiter, selector)
        {
            _selector = selector;
        }

        public string GetText()
        {
            return Element.Text;
        }

        public void ClickLinkInCellElement(int rowNumber)
        {
            Element.FindElement(By.CssSelector("a")).Click();
        }

        public void ClickLinkInCell()
        {
            Element.FindElement(By.CssSelector("a")).Click();
        }

        public void ClickButtonInCell()
        {
            Element.FindElement(By.CssSelector("input")).Click();
        }

        public void ClickCell()
        {
            Element.Click();
        }

        public T GetElementInCell<T>() where T : WebDriverArmControl
        {
            return (T)Activator.CreateInstance(typeof(T), Driver, Waiter, _selector);
        }

        public IWebElement GetElementWithinCell(string selector)
        {
            return Element.FindElement(By.CssSelector(selector));
        }

        public void EnterCellText(string name)
        {
            Element.Clear();
            Element.SendKeys(name);
        }

        public void EnterTextInToTextArea(string name)
        {
            Element.Click();
            var textarea = Element.FindElement(By.TagName("textarea"));
            textarea.Clear();
            textarea.SendKeys(name);
        }

        public void ClickCheckbox()
        {
            Element.Click();
            var mandatory = Element.FindElement(By.TagName("input"));
            mandatory.Click();
        }

        public void SearchName(string name)
        {
            Element.SendKeys(name);
        }
        
        public void SelectRowName(string name)
        {
            var rowSelector = CssSelectorString + " table tr.Row td[align='left'] div";
            var elements = Driver.FindElements(By.CssSelector(rowSelector));
            foreach (var ele in elements)
            {
                if (ele.Text == name)
                {
                   ele.Click();
                }                           
            }
        }

        public string GetSelectedValue()
        {
            return Element.GetAttribute("title");
        }

      
        public void SelectValue(string value)
        {
            WaitForElementToAppear();
            SelectElement select = new SelectElement(Element);
            Assert.True(select.Options.Select(o => o.Text).Contains(value));
            select.SelectByText(value);
        }

    }

    public class WebDriverTickBoxTableCell : WebDriverTableCell
    {
        private readonly IWebElement _checkbox;

        public WebDriverTickBoxTableCell(IWebDriver driver, WebDriverWait waiter, string id) : base(driver, waiter, id)
        {
            SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(CssSelectorString + " input[type='checkbox']"));
            _checkbox = Driver.FindElement(By.CssSelector(CssSelectorString + " input[type='checkbox']"));
        }

        public void Check()
        {
            if (!_checkbox.Selected)
                _checkbox.Click();
        }

        public void Uncheck()
        {
            if (_checkbox.Selected)
                _checkbox.Click();
        }

        public void IsDisabled()
        {
            Assert.False(_checkbox.Enabled);
        }

        public void IsEnabled()
        {
            Assert.True(_checkbox.Enabled);
        }

        public void AssertChecked()
        {
            Assert.True(_checkbox.Selected);
        }

        public void AssertUnchecked()
        {
            Assert.False(_checkbox.Selected);
        }
    }
}
