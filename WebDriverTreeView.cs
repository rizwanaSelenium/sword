using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverTreeView : WebDriverArmControl
    {
        public WebDriverTreeNode Root { get; set; }

        public WebDriverTreeView(IWebDriver driver, WebDriverWait waiter, string id)
            : base(driver, waiter, "div#" + id)
        {

            Root = new WebDriverTreeNode(driver, waiter, Element.FindElement(By.CssSelector("div.tree-root")).GetAttribute("id"));
        }

        public void CheckNodes(params string[] items)
        {
            items.ForEach(i => Root.GetTreeNode(i).CheckNode());
        }

        public void SelectNode(string locator)
        {
            Root.GetTreeNode(locator).SelectNode();
        }

        public void AssertSelectedValues(params string[] items)
        {
           items.ForEach(i => Assert.True(Root.GetTreeNode(i).Checked(), "Expected " + i + " to be checked, but it was not."));
        }
    }
}
