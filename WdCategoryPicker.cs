using System;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WdCategoryPicker : WebDriverArmControl
    {
        private readonly string _id;

        private void MoveToCenterOfCategoryPickerTree()
        {
            var categoriesPopup = Element.FindElement(By.CssSelector("div.tree-root"));
            var actions = new Actions(Driver);
            actions.MoveToElement(categoriesPopup).Build().Perform();
        }

        public WdCategoryPicker(IWebDriver driver, WebDriverWait waiter, string id)
            : base(driver, waiter, "div#" + id)
        {
            _id = id;
            LabelElement = Driver.FindElement(By.CssSelector(CssSelectorString + " div.lbl"));
        }

        public void ExpandItem(string itemToExpand, bool treeExpanded = false)
        {
            bool optionFound = false;

            // Click Selector to display options
            if (!treeExpanded)
            {
                Element.Click();
            }

            // Keep tree open by moving to the centre of it
            MoveToCenterOfCategoryPickerTree();

            // Find all options within the Category Picker that begin with div#treenode_
            // These are our Category Options to search through
            var options = Element.FindElements(By.CssSelector("div[id^='treenode_']"));

            for (int i = 0; i < 30; i++)
            {
                if (options.Any())
                {
                    treeExpanded = true;
                    break;
                }
                Element.Click();
                // Keep tree open by moving to the centre of it
                MoveToCenterOfCategoryPickerTree();
                options = Element.FindElements(By.CssSelector("div[id^='treenode_']"));
            }
            // If the category picker tree never opens
            if (!treeExpanded)
            {
                Assert.Fail("Category picker tree failed to open.");
            }

            // Check if each option has the option we are looking for and if it's not
            // already selected, then select it
            foreach (var option in options)
            {
                if (option.FindElement(By.CssSelector("span.tree-label")).Text.Equals(itemToExpand))
                {
                    option.FindElement(By.ClassName("tree-expander")).Click();
                    optionFound = true;
                    break;
                }
            }

            // If we don't find the option then fail
            if (!optionFound)
            {
                Assert.Fail("Option: " + itemToExpand + " Not Found");
            }
        }

        public void SelectItem(string itemToSelect, bool treeExpanded = false)
        {
            bool optionFound = false;

            // Click Selector to display options
            if (!treeExpanded)
            {
                Element.Click();
            }

            // Keep tree open by moving to the centre of it
            MoveToCenterOfCategoryPickerTree();

            // Find all options within the Category Picker that begin with div#treenode_
            // These are our Category Options to search through
            var options = Element.FindElements(By.CssSelector("div[id^='treenode_']"));

            for (int i = 0; i < 30; i++)
            {
                if (options.Any())
                {
                    treeExpanded = true;
                    break;
                }
                Element.Click();
                // Keep tree open by moving to the centre of it
                MoveToCenterOfCategoryPickerTree();
                options = Element.FindElements(By.CssSelector("div[id^='treenode_']"));
            }
            // If the category picker tree never opens
            if (!treeExpanded)
            {
                Assert.Fail("Category picker tree failed to open.");
            }


            // Check if each option has the option we are looking for and if it's not
            // already selected, then select it
            foreach (var option in options)
            {
                if (option.FindElement(By.CssSelector("span.tree-label")).Text.Equals(itemToSelect))
                {
                    if (!option.FindElement(By.CssSelector("input[type='checkbox']")).Selected)
                    {
                        option.FindElement(By.CssSelector("input[type='checkbox']")).Click();
                    }
                    optionFound = true;

                    break;
                }
            }

            // Click selector again to close the options
            Element.FindElement(By.CssSelector("ul")).Click();

            // If we don't find the option then fail
            if (!optionFound)
            {
                Assert.Fail("Option: " + itemToSelect + " Not Found");
            }
        }


        public void CloseCategoryPicker()
        {
            //Closing the list after all selected as it can get in the way if not closed
            Element.Click();
        }

        public new void AssertMandatory()
        {
            Assert.True(Element.FindElement(By.CssSelector("ul[id^='" + _id + "']")).GetAttribute("style").ToLower().Contains("background-color: #ffefd5") || Element.FindElement(By.CssSelector("ul[id^='" + _id + "']")).GetAttribute("style").ToLower().Contains("background-color: rgb(255, 239, 213)"), "Expected Element {0} {1} to be mandatory, but it was not", CssSelectorString, Element.GetAttribute("style"));
        }

        public new void AssertReadOnly()
        {
            try
            {
                Assert.True(Element.FindElement(By.CssSelector("div#" + _id + "_TreeView")).GetAttribute("unselectable").ToLower().Equals("on"), "Expected Element {0} to be read only, but it was not", CssSelectorString);
            }
            catch (Exception e)
            {
                throw new Exception("Expected Element " + CssSelectorString + e + " to be read only, but it was not");
            }
        }
    }
}
