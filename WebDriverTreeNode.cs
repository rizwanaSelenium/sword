using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverTreeNode : WebDriverArmControl
    {
        public string Label { get; set; }

        public WebDriverTreeNode(IWebDriver driver, WebDriverWait waiter, string id) : base(driver, waiter, "div.treenode#" + id)
        {
            WaitForTabPageToBeReady();
            Label = Element.FindElement(By.CssSelector("span.tree-label")).Text;
        }

        public bool IsExpanded()
        {
            return Element.FindElement(By.CssSelector("img.tree-expander")).GetAttribute("src").Contains("treeminus.gif");
        }

        public bool IsCollapsed()
        {
            return Element.FindElement(By.CssSelector("img.tree-expander")).GetAttribute("src").Contains("treeplus.gif");
        }

        public void Expand()
        {
            if(IsCollapsed())
                Element.FindElement(By.CssSelector("img.tree-expander")).Click();
        }

        public void SelectNode()
        {
            Element.FindElement(By.CssSelector("span.tree-label")).Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        public void CheckNode()
        {
            var checkbox = Element.FindElement(By.CssSelector("input.tree-checkbox"));

            if(!checkbox.Selected)
                checkbox.Click();
        }

        public void UncheckNode()
        {
            var checkbox = Element.FindElement(By.CssSelector("input.tree-checkbox"));

            if (checkbox.Selected)
                checkbox.Click();
        }

        public bool Checked()
        {
            return Element.FindElement(By.CssSelector("input.tree-checkbox")).Selected;
        }

        public WebDriverTreeNode GetTreeNode(string fullyQualifiedName)
        {
            if (fullyQualifiedName.Trim() == Label)
                return this;

            if (fullyQualifiedName.Split('>').First().Trim() == Label)
            {
                Expand();
                foreach (var child in GetChildren())
                {
                    var found = child.GetTreeNode(string.Join(">", fullyQualifiedName.Split('>').Skip(1)));
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        public void Collapse()
        {
            if(IsExpanded())
                Element.FindElement(By.CssSelector("img.tree-expander")).Click();
        }

        public List<WebDriverTreeNode> GetChildren()
        {
            var children = new List<WebDriverTreeNode>();

           Element.Click();
            Element.FindElements(By.CssSelector("div.treenode")).ForEach(child => 
                children.Add(new WebDriverTreeNode(Driver, Waiter, child.GetAttribute("id"))));

            return children;
        }
        
        public void SelectNodeByName(string name)
        {
            var ele = Element.FindElements(By.CssSelector("span"));
            foreach (var option in ele)
                if (option.Text.Equals(name))
                {
                    option.Click();
                }
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

         public void AssertTreeNode(string nodeName)
         {
             
            var foldersList = Element.FindElements(By.CssSelector("span.tree-label")).ToList();

             foreach (var option in foldersList)
                 if (option.Text.Equals(nodeName))
                     break;
             
           }

        public List<IWebElement> GetElementsOfNodeToDeleteByName(string name)
        {
            var ele = Element.FindElements(By.CssSelector("div.treenode span"));
            List<IWebElement> titeList = new List<IWebElement>();
            
            foreach(var option in ele)
                if (option.Text.Equals(name))
                {
                    var value = option.GetAttribute("title");
                    IWebElement nodeElement = Element.FindElement(By.CssSelector("span[title='" + value + "']"));
                    titeList.Add(nodeElement);

                }
            return titeList;
        }

        public void SetText(string text)
        {
            var textElement = Element.FindElement(By.CssSelector("span input"));
            textElement.Clear();
            textElement.SendKeys(text);
            
        }

        public string GetNodeByName()
        {
            var ele = Element.FindElement(By.CssSelector("div span"));
            var value = ele.Text;

            return value;

        }

        public void AssertTreeNodeNotPresent(string nodeName)
        {

            var foldersList = Element.FindElements(By.CssSelector("span.tree-label")).ToList();

            foreach (var option in foldersList)
                if (option.Text.Equals(nodeName))
                    Assert.Fail("Node is still present");
        }
    }
}
