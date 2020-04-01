using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverTreeItemPicker : WebDriverArmControl
    {
        private readonly string _id;
        private WebDriverTreeView Tree
        {
            get
            {
                WaitForElementToAppear();
                Element.Click();
                return new WebDriverTreeView(Driver, Waiter, _id + "_TreeView");
            }
        }

        public WebDriverTreeItemPicker(IWebDriver driver, WebDriverWait waiter, string id) : base(driver, waiter, "div#" + id)
        {
            _id = id;
            LabelElement = Driver.FindElement(By.CssSelector(CssSelectorString + " div.lbl"));
        }

        public void SetValue(params string[] items)
        {
            Tree.CheckNodes(items);
        }

        public void AssertSelectedValues(params string[] items)
        {
            Tree.AssertSelectedValues(items);
        }

        public override void AssertTooltipEquals(string text)
        {
            Assert.AreEqual(text, Element.FindElement(By.CssSelector("ul")).GetAttribute("title"));
        }
    }
}
