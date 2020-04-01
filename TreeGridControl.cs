using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class TreeGridControl : WebDriverArmControl
    {
        public TreeGridControl(IWebDriver driver, WebDriverWait waiter, string id) : base(driver, waiter, "td#" + id + "_treeCol")
        {

        }

        public void SelectNode(int itemId)
        {
            var checkbox = new WebDriverTickBoxControl(Driver, Waiter, CssSelectorString + string.Format(" div.treenode[data-itemid='{0}'] input", itemId), true);
            checkbox.Check();
        }

        public void DeselectNode(int itemId)
        {
            var checkbox = new WebDriverTickBoxControl(Driver, Waiter, CssSelectorString + string.Format(" div.treenode[data-itemid='{0}'] input", itemId), true);
            checkbox.Uncheck();
        }

        public void AssertUnchecked(int itemId)
        {
            var checkbox = new WebDriverTickBoxControl(Driver, Waiter, CssSelectorString + string.Format(" div.treenode[data-itemid='{0}'] input", itemId), true);
            checkbox.AssertUnchecked();
        }

        public void ExpandTreeModule()
        {
            if (Element.FindElement(By.CssSelector("img.tree-expander")).GetAttribute("src").Contains("treeplus.gif"))
                Element.FindElement(By.CssSelector("img.tree-expander")).Click();
        }
    }
}
