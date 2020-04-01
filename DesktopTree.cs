using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class DesktopTree : WebDriverArmControl
    {
        private DesktopTreeNode _businessAreaNode;
        public DesktopTreeNode BusinessAreaNode
        {
            get
            {
                return _businessAreaNode ?? (_businessAreaNode = new DesktopTreeNode(Driver, Waiter, ".treenode.tree-root"));
            }
        }

        public DesktopTree(IWebDriver driver, WebDriverWait waiter, string id) : base(driver, waiter, string.Format("div#AV_{0}_TreeGrid div#AV_{0}_TreeGrid_Tree", id))
        {
            //also wait for the Business Area Node to appear
            Waiter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(CssSelectorString + " div.treenode.tree-root")));
        }

        public void ExpandItem(DesktopTreeType itemType, string itemName)
        {
            if (!FindTreeItems(itemType, itemName).First().FindElement(By.CssSelector("img.tree-expander")).GetAttribute("src").Contains("treeminus.gif"))
            {
                FindTreeItemAndThen(itemType, itemName, item =>
                {
                    item.Expand();
                });
            }
        }

        private List<string> GetImageNameFromTreeType(DesktopTreeType thingType)
        {
            var imageNames = new List<string>();
            switch (thingType)
            {
                case DesktopTreeType.Folder:
                    imageNames.Add("folderyellow.gif");
                    imageNames.Add("folderblue.gif");
                    break;
                case DesktopTreeType.Module:
                    imageNames.Add("folderyellow.gif");
                    break;
                case DesktopTreeType.Activity:
                    imageNames.Add("flagyellow.gif");
                    break;
                case DesktopTreeType.Process:
                    imageNames.Add("flaskyellow.gif");
                    break;
                case DesktopTreeType.CostBreakdown:
                case DesktopTreeType.Escalation:
                case DesktopTreeType.FinancialAccount:
                case DesktopTreeType.Asset:
                case DesktopTreeType.Kpi:
                case DesktopTreeType.Requirement:
                    imageNames.Add("document.gif");
                    break;
                default:
                    throw new ArgumentException(string.Format("Could not find an image name for tree type {0}", thingType));
            }

            return imageNames;
        }

        private List<IWebElement> FindTreeItems(DesktopTreeType thingType, string thingName)
        {
            WaitForTabPageToBeReady();
            foreach (var imageName in GetImageNameFromTreeType(thingType))
            {
                var things = Element.FindElements(By.CssSelector(string.Format("img[src*='{0}'] + span.tree-label", imageName)))
                    .Where(f => f.Text == thingName).ToList();
                if (things.Any())
                {
                    things = things.Select(f => f.FindElement(By
                            .XPath(".."))) //select the parent element of the image, i.e. the div that contains the label, the image and the expander.
                        .ToList();

                    return things;
                }
            }

            return new List<IWebElement>();
        }

        private void FindTreeItemAndThen(DesktopTreeType thingType, string thingName, Action<DesktopTreeNode> action)
        {
            var things = FindTreeItems(thingType, thingName);

            Assert.AreEqual(1, things.Count, "Expected there to be one {0} in the tree with name {1} but there was {2}.", thingType, thingName, things.Count);

            action(new DesktopTreeNode(Driver, Waiter, things.Single()));
        }

        private void FindTreeItemsAndThen(DesktopTreeType thingType, string thingName, Action<List<DesktopTreeNode>> action)
        {
            var things = FindTreeItems(thingType, thingName);

            action(things.Select(f => new DesktopTreeNode(Driver, Waiter, f)).ToList());
        }

        public void OpenItemPropertiesFromContextMenu(DesktopTreeType itemType, string itemName)
        {
            FindTreeItemAndThen(itemType, itemName, item =>
            {
                item.Click();
                WaitUntilUiSpinnerIsNotDisplayed();
                item.RightClick();
                SelectContextMenuItem("Properties");

                Driver.WaitForAjaxToComplete();
            });
        }

        public void OpenItemDataHistoryFromContextMenu(DesktopTreeType itemType, string itemName)
        {
            FindTreeItemAndThen(itemType, itemName, item =>
            {
                item.RightClick();
                SelectContextMenuItem("ViewDataHistory");

                Waiter.Until(d => !d.IsAjaxRequestInProgress());
            });
        }

        public void CutItemFromEditMenu(DesktopTreeType itemType, string itemName)
        {
            FindTreeItemAndThen(itemType, itemName, item =>
            {
                item.Click();

                SelectCutNodeFromEditMenu();
            });
        }

        public void DeleteItemFromEditMenu(DesktopTreeType itemType, string itemName)
        {
            FindTreeItemAndThen(itemType, itemName, item =>
            {
                item.Click();

                SelectDeleteNodeFromEditMenu();
            });
        }

        public void PasteItemAschildFromEditMenu(DesktopTreeType itemType, string itemName)
        {
            FindTreeItemAndThen(itemType, itemName, item =>
            {
                item.Click();

                SelectPasteAsChildNodeFromEditMenu();
                Thread.Sleep(500);
                Waiter.Until(d => !d.IsAjaxRequestInProgress());
            });
        }


        public void DeleteCopiedItemFromEditMenu(DesktopTreeType itemType, string itemName)
        {
            FindTreeItemsAndThen(itemType, itemName, items =>
            {
                Assert.AreEqual(2, items.Count, "Expected there to be two folders with name {0} but there was {1}.", itemName, items.Count);

                items.Last().Click();
                SelectDeleteNodeFromEditMenu();
            });
        }

        public void DeleteItemFromContextMenu(DesktopTreeType itemType, string itemName)
        {
            FindTreeItemAndThen(itemType, itemName, item =>
            {
                item.Click();
                item.RightClick();
                SelectContextMenuItem("DeleteNode");
            });
        }

        public void OpenCopiedItemPropertiesFromViewMenu(DesktopTreeType itemType, string itemName)
        {
            FindTreeItemsAndThen(itemType, itemName, items =>
            {
                Assert.AreEqual(2, items.Count, "Expected there to be two items of type {0} with name {1} but there was {2}.", itemType, itemName, items.Count);
                items.Last().Click();

                SelectPropertiesFromViewMenu();
            });
        }

        public void OpenItemPropertiesFromViewMenu(DesktopTreeType itemType, string itemName)
        {
            FindTreeItemAndThen(itemType, itemName, item =>
            {
                item.Click();

                SelectPropertiesFromViewMenu();
            });
        }

        public void ClickItem(DesktopTreeType itemType, string itemName)
        {
            FindTreeItemAndThen(itemType, itemName, item =>
            {
                item.Click();
            });
            WaitUntilDesktopFooterIsDisplayed();
        }

        public void RightClickOnItem(DesktopTreeType itemType, string itemName)
        {
            FindTreeItemAndThen(itemType, itemName, item =>
            {
                item.RightClick();
            });
        }

        public void AssertItemDoesNotExist(DesktopTreeType type, string name)
        {
            FindTreeItemsAndThen(type, name, items =>
            {
                Assert.False(items.Any());
            });
        }

        public void AssertItemExist(DesktopTreeType type, string name)
        {
            FindTreeItemsAndThen(type, name, items =>
            {
                Assert.True(items.Any());
            });

        }
        private void SelectContextMenuItem(string menuItemToSelect)
        {
            IWebElement contextMenuItem = Driver.FindElement(By.CssSelector("li[id$='_TreeGrid_ContextMenu_" + menuItemToSelect + "']"));
            Waiter.Until(d => contextMenuItem.Displayed);
            contextMenuItem.Click();
        }

        private void SelectDeleteNodeFromEditMenu()
        {
            var menu = new WebDriverMenu(Driver, Waiter, "AV_Menu_menu");
            var editMenu = menu.GetMenuItemById("MenuEdit");
            editMenu.Click();
            editMenu.ClickOption("Delete Node");
        }

        private void SelectCutNodeFromEditMenu()
        {
            var menu = new WebDriverMenu(Driver, Waiter, "AV_Menu_menu");
            var editMenu = menu.GetMenuItemById("MenuEdit");
            editMenu.Click();
            editMenu.ClickOption("Cut Node");
        }
        private void SelectPasteAsChildNodeFromEditMenu()
        {
            var menu = new WebDriverMenu(Driver, Waiter, "AV_Menu_menu");
            var editMenu = menu.GetMenuItemById("MenuEdit");
            editMenu.Click();
            editMenu.ClickOption("Paste As Child");
        }
        private void SelectPropertiesFromViewMenu()
        {
            var menu = new WebDriverMenu(Driver, Waiter, "AV_Menu_menu");
            var viewMenu = menu.GetMenuItemById("MenuView");
            viewMenu.Click();
            viewMenu.ClickOptionFromMenu("Properties");
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        public void AssertItemHighlighted(DesktopTreeType itemType, string itemName)
        {
            FindTreeItemAndThen(itemType, itemName, item =>
            {
                item.AssertIsHighlighted();
            });
        }

        public void LinkRiskToFolderFromContextMenu(string folderName)
        {
            FindTreeItemAndThen(DesktopTreeType.Activity, folderName, folder =>
            {
                folder.Click();
                folder.RightClick();
                SelectContextMenuItem("LinkSelectedRecordsToSelectedNode");
            });
        }

        public void LinkRiskToFolder(string folderName)
        {
            FindTreeItemAndThen(DesktopTreeType.Folder, folderName, folder =>
            {
                folder.Click();
                folder.RightClick();
                SelectContextMenuItem("LinkSelectedRecordsToSelectedNode");
            });
        }

        public void LinkRiskToActivity(string folderName)
        {
            FindTreeItemAndThen(DesktopTreeType.Activity, folderName, folder =>
            {
                folder.Click();
                folder.RightClick();
                SelectContextMenuItem("LinkSelectedRecordsToSelectedNode");
            });
        }

        public void LinkRiskToProcess(string folderName)
        {
            FindTreeItemAndThen(DesktopTreeType.Process, folderName, folder =>
            {
                folder.Click();
                folder.RightClick();
                SelectContextMenuItem("LinkSelectedRecordsToSelectedNode");
            });
        }

        public void RightClickOnFolderAndSelectMenuOption(DesktopTreeType treeItemType, string folderName, string optionToSelect)
        {
            FindTreeItemAndThen(treeItemType, folderName, folder =>
            {
                folder.Click();
                folder.RightClick();
                SelectContextMenuItem(optionToSelect);
            });
        }


        public void WaitUntilTreeItemDeleted(DesktopTreeType type, string name)
        {
            bool itemDeleted = false;
            for (var i = 0; i < 30; i++)
            {
                Thread.Sleep(1000);
                var items = FindTreeItems(type, name);
                if (items.Count == 0)
                {
                    itemDeleted = true;
                    break;
                }
            }
            if (!itemDeleted)
            {
                Assert.Fail("Item {0} with name {1} is still present in the tree when it was expected to be deleted.",
                    type, name);
            }
        }

        public void WaitUntilCopiedTreeItemDeleted(DesktopTreeType type, string name)
        {
            bool copiedItemDeleted = false;
            for (var i = 0; i < 30; i++)
            {
                Thread.Sleep(1000);

                var items = FindTreeItems(type, name);
                if (items.Count == 1)
                {
                    copiedItemDeleted = true;
                    break;
                }
            }
            if (!copiedItemDeleted)
            {
                Assert.Fail("Copied item {0} with name {1} is still present in the tree when it was expected to be deleted.",
                    type, name);
            }
        }
    }



    public class DesktopTreeNode : WebDriverArmControl
    {
        private readonly WebDriverArmControl _expander;

        //Made this constructor as the TestCreateProcessFromViewMenu test would fail randomly for strange stale element reasons
        public DesktopTreeNode(IWebDriver driver, WebDriverWait waiter, string outerContainer)
            : base(driver, waiter, null)
        {
            WaitUntilUiSpinnerIsNotDisplayed();

            string outerContainerIdAttribute = driver.FindElement(By.CssSelector(outerContainer)).GetAttribute("id");

            if (outerContainerIdAttribute == null)
                throw new Exception(string.Format("Error getting attribute of the desktop tree node '{0}' .", GetText()));

            _expander = new WebDriverArmControl(Driver, waiter, "#" + outerContainerIdAttribute + " img.tree-expander");

            SetSelectorString("#" + outerContainerIdAttribute + " span.tree-label");
        }

        public DesktopTreeNode(IWebDriver driver, WebDriverWait waiter, IWebElement outerContainer) : base(driver, waiter, null)
        {
            WaitUntilUiSpinnerIsNotDisplayed();
            string outerContainerIdAttribute = outerContainer.GetAttribute("id");

            if (outerContainerIdAttribute == null)
                throw new Exception(string.Format("Error getting attribute of the desktop tree node '{0}' .", GetText()));

            _expander = new WebDriverArmControl(Driver, waiter, "#" + outerContainerIdAttribute + " img.tree-expander");

            SetSelectorString("#" + outerContainerIdAttribute + " span.tree-label");
        }

        public void RightClick()
        {
            Driver.RightClickOnElement(Element);
        }

        public string GetText()
        {
            return Element.Text;
        }

        public void AssertIsHighlighted()
        {
            Assert.True(Element.GetAttribute("class").Contains("highlightGreen"));
        }

        public void WaitUntilNodeIsExpanded()
        {
            var nodeExpanded = false;
            var errorsThrown = 0;
            for (var i = 0; i < 60; i++)
            {
                try
                {
                    Thread.Sleep(500);
                    if (_expander.GetAttribute("src").Contains("treeminus.gif"))
                    {
                        nodeExpanded = true;
                        break;
                    }
                }
                catch (Exception)
                {
                    errorsThrown++;
                }
            }

            if (errorsThrown != 0)
            {
                Console.WriteLine("Errored checking if node was expanded " + errorsThrown + " times.");
            }
            if (!nodeExpanded)
            {
                Assert.Fail("Node was not expanded. Node I was waiting for: " + GetText());
            }
        }

        public void Expand()
        {
            if (_expander == null)
                throw new Exception(string.Format("Could not expand element '{0}' - no expander element was found.", GetText()));

            if (_expander.GetAttribute("src").Contains("treeplus.gif"))
            {
                _expander.Click();
               WaitUntilNodeIsExpanded();
            }
        }

        public void ExpandFolder()
        {
            var treenodeTitle = Element.GetAttribute("title");
            var splitTreenodeTitle = treenodeTitle.Split(null);
            var treenodeId = splitTreenodeTitle[2];
            if (Driver.FindElement(By.CssSelector("div#treenode_" + treenodeId + "_div img.tree-expander")).GetAttribute("src").Contains("treeplus.gif"))
            {
                Driver.FindElement(By.CssSelector("div#treenode_" + treenodeId + "_div img.tree-expander")).Click();
                WaitUntilUiSpinnerIsNotDisplayed();
            }
        }

        public override void Click()
        {
            Element.Click();

                for (var i = 0; i < 240; i++)
                {
                    try
                    {
                        Thread.Sleep(1000);
                        if (Element.GetAttribute("class").Contains("treenode-selected"))
                        {
                            break;
                        }

                        Element.Click();
                    }
                    catch (NoSuchElementException)
                    {
                        if (i == 239)
                        {
                            throw new NoSuchElementException("Failed on Click in DesktopTree.cs - Failed to click on desktop tree element not found. Element wanted - " + Element);
                        }
                    }
                }

            WaitUntilUiSpinnerIsNotDisplayed();
            WaitUntilDesktopFooterIsEnabled();
        }
    }
}
