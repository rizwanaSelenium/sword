using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Keys = OpenQA.Selenium.Keys;

namespace PresentationModel.Controls
{
    public class WebDriverTableControl : WebDriverArmControl
    {
        private readonly string _id;
        private readonly string _cssSelectorString;
        private readonly string _tableSelectorString;
        public DesktopGridLocators GridLocators;

        public WebDriverTableControl(IWebDriver driver, WebDriverWait waiter, string id) : base(driver, waiter, "div.tablecontrol#" + id)
        {
            _id = id;

            _cssSelectorString = "div.tablecontrol#" + id;
            _tableSelectorString = CssSelectorString + " div.content-div table.grid-table";
        }

        public void AssertNumberOfItemsDisplayed(int numberOfItemsExpected)
        {
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));
            Assert.AreEqual(numberOfItemsExpected, records.Count, "Unexpected number of items displayed. Expected " + numberOfItemsExpected + " but found " + records.Count);
        }

        public bool CheckRowCountIsZero()
        {
            var tableElement = Driver.FindElements(By.CssSelector(_tableSelectorString)).SingleOrDefault(t => t.Displayed);
            if (tableElement == null)
            {
                return true;
            }
            try
            {
                Element.FindElement(By.CssSelector("div.grid-norecords"));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
         }

        // This counts the rows in the currently active table page that have non-null items in them
        // This will take A LONG WHILE to complete on larger tables e.g. the Allowed / Not Allowed
        // Roles Tables.  If you wish to get the number of rows for a table, use the GetTotalRowCount()
        public int GetPageRowCount()
        {
            var rows = Element.FindElements(By.CssSelector("tbody tr.grid-row"));

            return rows.Count(r => r.FindElements(By.CssSelector("td.grid-cell"))
                .Any(c => !string.IsNullOrEmpty(c.Text.Trim())));
        }

        public int GetTotalRowCount()
        {
            try
            {
                Driver.FindElement(By.CssSelector(CssSelectorString + " div.grid-footer#" + _id + "_pagerDiv" + " tr#" + _id + "_pagerRow" + " td span#" + _id + "_recordCountSpan"));
                var recordCount = Driver.FindElement(By.CssSelector(CssSelectorString + " div.grid-footer#" + _id + "_pagerDiv" + " tr#" + _id + "_pagerRow" + " td span#" + _id + "_recordCountSpan")).Text;
                return Convert.ToInt16(recordCount);
            }
            catch (NoSuchElementException)
            {
                var tableElement = Driver.FindElements(By.CssSelector(_tableSelectorString)).SingleOrDefault(t => t.Displayed);
                if (tableElement == null)
                {
                    return 0;
                }
                var rows = Element.FindElements(By.CssSelector("tbody tr.grid-row"));
                return rows.Count;
            }
        }

        public void SelectAll()
        {
            Waiter.Until(d => d.FindElement(By.CssSelector("input#" + _id + "_selectAll"))).Click();
        }

        public void SelectAllByUsingSpaceKey()
        {
            Waiter.Until(d => d.FindElement(By.CssSelector("input#" + _id + "_selectAll"))).SendKeys(Keys.Space);
        }

        public void NextPage()
        {
            Waiter.Until(d => d.FindElement(By.CssSelector("img#" + _id + "_nextImg"))).Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        public void PreviousPage()
        {
            Waiter.Until(d => d.FindElement(By.CssSelector("img#" + _id + "_prevImg"))).Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        public void FirstPage()
        {
            Waiter.Until(d => d.FindElement(By.CssSelector("img#" + _id + "_firstImg"))).Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        public void LastPage()
        {
            Waiter.Until(d => d.FindElement(By.CssSelector("img#" + _id + "_lastImg"))).Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        public void GoToPage(int pageNumber)
        {
            var textBox = Waiter.Until(d => d.FindElement(By.CssSelector("input#" + _id + "_pagerText")));
            textBox.Clear();
            textBox.SendKeys(pageNumber.ToString());
        }

        public void Delete()
        {
            Waiter.Until(d => d.FindElement(By.CssSelector("td#" + _id + "_delImg"))).Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        public void Search(string searchString)
        {
            Waiter.Until(d => d.FindElement(By.CssSelector("input#" + _id + "_searchBox"))).Clear();
            Waiter.Until(d => d.FindElement(By.CssSelector("input#" + _id + "_searchBox"))).SendKeys(searchString);
            Waiter.Until(d => d.FindElement(By.CssSelector("td#" + _id + "_searchImg"))).Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        public T ClickCellToOpenDialog<T>(int colNumber, int rowNumber) where T : WebDriverArmPage
        {
            FindCellThen(rowNumber, colNumber, cell => cell.ClickLinkInCell());

            return (T) Activator.CreateInstance(typeof (T), Driver, Waiter);
        }

        public T ClickButtonToOpenDialog<T>() where T : WebDriverArmPage
        {           
          var cell=  Driver.FindElement(By.Id("0_TargetRiskLevel"));
          cell.FindElement(By.XPath(".//button")).Click();            
           var child = (T)Activator.CreateInstance(typeof(T), Driver, Waiter);
         return child;
        }

        public T ClickCellButtonToOpenDialog<T>(int colNumber, int rowNumber) where T : WebDriverArmPage
        {
            FindCellThen(rowNumber, colNumber, cell => cell.ClickButtonInCell());

            var child = (T)Activator.CreateInstance(typeof(T), Driver, Waiter);
            return child;
        }

        private void FindCellThen(int rowNumber, int colNumber, Action<WebDriverTableCell> action)
        {
            var rows = Element.FindElements(By.CssSelector("tr.grid-row"));
            if (rows.Count == 0)
            {
                Assert.Fail("Table has 0 rows");
            }
            if (rowNumber >= rows.Count)
                Assert.Fail("Table only has " + rows.Count + " rows");

            var cells = rows[rowNumber].FindElements(By.CssSelector("td.grid-cell"));
            if (colNumber >= cells.Count)
                Assert.Fail("Row only has " + cells.Count + " columns");

            //Start of ID escape modification
            //This was added as a selenium update now means that rules are being applied to ID's starting with a number when using a css-selector
            //ID's starting with a number have to be escaped this is what the \3 is for, we also add a space after our first number as this would
            //be needed for where our ID has two or more numbers at the start of the ID.
            //An example is an ID of 0_Name becomes \30 _Name, and 12_Name would become \31 2_Name.

            var tableCellIdModifier = cells[colNumber].GetAttribute("id");
            Char[] escapeIdStartingWithNumber = tableCellIdModifier.ToCharArray();
            if (Char.IsNumber(escapeIdStartingWithNumber[0]))
            {
                tableCellIdModifier = tableCellIdModifier.Insert(0, "\\3");
                tableCellIdModifier = tableCellIdModifier.Insert(3, " ");
            }
            var cell = new WebDriverTableCell(Driver, Waiter, _tableSelectorString + " td.grid-cell#" + tableCellIdModifier);

            //End of ID escape modification

            //old method encase we need it back
            action(cell);
        }

        public void ClickCellButtonToUnlink(int rowNumber, int colNumber)
        {
            FindCellThen(rowNumber, colNumber, row => row.ClickButtonInCell());
         }

        private void FindCellToEnterText(int rowNumber, int colNumber, Action<WebDriverTableCell> action)
        {
            var rows = Element.FindElements(By.CssSelector("tr.grid-row"));
            if (rowNumber >= rows.Count)
                Assert.Fail("Table only has " + rows.Count + " rows");

            var cells = rows[rowNumber].FindElements(By.CssSelector("td.grid-cell"));
            if (colNumber >= cells.Count)
                Assert.Fail("Row only has " + cells.Count + " columns");

            var cell = new WebDriverTableCell(Driver, Waiter, _tableSelectorString + " td.grid-cell#" + cells[colNumber].GetAttribute("id") + " input");
            action(cell);
        }

        public void AssertCellText(int rowNumber, int colNumber, string expected)
        {
            FindCellThen(rowNumber, colNumber, cell => Assert.AreEqual(expected, cell.GetText(), "Expected cell ({0},{1}) in table {2} to have exact text '{3}' but was '{4}'.", rowNumber, colNumber, _id, expected, cell.GetText()));
        }

        public void AssertCellTextContains(int rowNumber, int colNumber, string expected)
        {
            FindCellThen(rowNumber, colNumber, cell => Assert.True(cell.GetText().Contains(expected), "Expected cell ({0},{1}) in table {2} to contain text '{3}' but cell text was '{4}'.", rowNumber, colNumber, _id, expected, cell.GetText()));
        }

        public void AssertCellTitle(int rowNumber, int colNumber, string expected)
        {
            FindCellThen(rowNumber, colNumber, cell => Assert.AreEqual(expected, cell.GetSelectedValue(), "Expected cell ({0},{1}) in table {2} to have tooltip '{3}' but it was '{4}'", rowNumber, colNumber, _id, expected, cell.GetSelectedValue()));   
        }

        public string GetCellText(int rowNumber, int colNumber)
        {
            var rows = Element.FindElements(By.CssSelector("tr.grid-row"));
            if (rows.Count == 0)
            {
                Assert.Fail("Table has 0 rows");
            }
            if (rowNumber >= rows.Count)
                Assert.Fail("Table only has " + rows.Count + " rows");

            var cells = rows[rowNumber].FindElements(By.CssSelector("td.grid-cell"));
            if (colNumber >= cells.Count)
                Assert.Fail("Row only has " + cells.Count + " columns");

            var cell = new WebDriverTableCell(Driver, Waiter, _tableSelectorString + " td.grid-cell#" + cells[colNumber].GetAttribute("id"));
            var cellValue = cell.GetText();
            return cellValue;
            
        }

        private void FindRowThen(int rowIndex, Action<WebDriverTableRow> action)
        {
            var rows = Element.FindElements(By.CssSelector("tr.grid-row"));
            if (rows.Count == 0)
            {
                Assert.Fail("Table has 0 rows");
            }
            if (rowIndex >= rows.Count)
                Assert.Fail("Table does not have that many rows");

            var row = new WebDriverTableRow(Driver, Waiter, rows[rowIndex].GetAttribute("index"), _tableSelectorString + " ");
            action(row);
        }

        public void CheckRow(int rowNumber)
        {
            FindRowThen(rowNumber, row => row.CheckRow());
        }

        public void UncheckRow(int rowNumber)
        {
            FindRowThen(rowNumber, row => row.UncheckRow());
        }

        public void CheckRows(params int[] rowNumbers)
        {
            rowNumbers.ForEach(CheckRow);
        }

        public void UncheckRows(params int[] rowNumbers)
        {
            rowNumbers.ForEach(UncheckRow);
        }

        public void ClickRow(int rowIndex)
        {
            FindRowThen(rowIndex, row => row.ClickRow());
        }

        public void ClickFirstRowEntry(int colToClick = 0)
        {
            if (colToClick == 0)
            {
                Driver.FindElement(By.CssSelector(CssSelectorString + " tr.grid-row td")).Click();
                return;
            }
            
            var cells = Driver.FindElements(By.CssSelector(CssSelectorString + " tr.grid-row td"));

            cells[colToClick].Click();
        }

        public void ClickRowEntryWithTitle(string title)
        {
            Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + title + "']")).Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        public void ClickRowEntryWithid(string id)
        {
            Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + id + "']")).Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        public void AssertRowEntryWithTitleIsNotPresent(string title)
        {
            var entriesInTable = Element.FindElements(By.CssSelector(CssSelectorString + " td[title='" + title + "']"));
            Assert.AreEqual(0, entriesInTable.Count, "Expected an entry with title {0} not to be present in the {1} table but it was", CssSelectorString, title);
        }

        public void AssertRowEntryWithIdIsNotPresent(string id)
        {
            var entriesInTable = Element.FindElements(By.CssSelector(CssSelectorString + " td[title='" + id + "']"));
            Assert.AreEqual(0, entriesInTable.Count, "Expected an entry with id {0} not to be present in the {1} table but it was", CssSelectorString, id);
        }

        public void EnterTextInToTextArea(int rowNumber, int colNumber, string name)
        {
            FindCellThen(rowNumber, colNumber, cell => cell.EnterTextInToTextArea(name));
        }

        public void ClickMandatoryCheckbox(int rowNumber, int colNumber)
        {
            FindCellThen(rowNumber, colNumber, cell => cell.ClickCheckbox());
        }

        public void ClickCheckboxByTitle(int rowNumber, int colNumber)
        {
            FindCellThen(rowNumber, colNumber, cell => cell.ClickCell());
        }
        public void ClickLinkInElement(int rowNumber, int colNumber)
        {
            FindCellThen(rowNumber, colNumber, cell => cell.ClickLinkInCell());
        }

        public void ClearSearch()
        {
            Waiter.Until(d => d.FindElement(By.CssSelector("td#" + _id + "_searchImg"))).Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }
   
        public void AssertSelectOption(int rowNumber, int colNumber, string expected)
        {
            FindCellToselecttext(rowNumber, colNumber, cell => Assert.AreEqual(expected, cell.GetSelectedValue()));
        }
       
        private void FindCellToselecttext(int rowNumber, int colNumber, Action<WebDriverTableCell> action)
        {
            var rows = Element.FindElements(By.CssSelector("tr.grid-row"));
            if (rowNumber >= rows.Count)
                Assert.Fail("Table only has " + rows.Count + " rows");

            var cells = rows[rowNumber].FindElements(By.CssSelector("td.grid-cell"));
            if (colNumber >= cells.Count)
                Assert.Fail("Row only has " + cells.Count + " columns");

            var cell = new WebDriverTableCell(Driver, Waiter,
                _tableSelectorString + " td.grid-cell#" + cells[colNumber].GetAttribute("id") + " select");
            action(cell);

        }
       
        public void PerformActionOnCellElementTextbox<T>(int rowNumber, int colNumber, Action<T> action)
            where T : WebDriverArmControl
        {
            FindCellToEnterText(rowNumber, colNumber, cell => action(cell.GetElementInCell<T>()));
        }

        public void PerformActionOnCellElementDropDown<T>(int rowNumber, int colNumber, Action<T> action)
            where T : WebDriverArmControl
        {
            FindCellToselecttext(rowNumber, colNumber, cell => action(cell.GetElementInCell<T>()));
        }

        public void ClickEntityInRow(string name, int rowN)
        {
            var ele = Element.FindElements(By.CssSelector("td[id^='" + rowN + "_'] a"));
            foreach (var e in ele)
            {
                if (e.Text == name)
                {
                    e.Click();
                    break;
                }
            }
        }

        public void GridClose()
        {
            var gridIcons = Element.FindElements(By.CssSelector("table#" + _id + "_title tbody tr#title td.grid-close"));
            if (gridIcons.Count > 1)
            {
                Assert.Fail(gridIcons.Count + "Grid Close Icons Found, Was Only Expecting 1");
            }
            Assert.True(gridIcons.Count == 1);
            gridIcons[0].Click();

        }

        public void RightClickRow(int rowNumber)
        {
            var rows = Element.FindElements(By.CssSelector("tr.grid-row"));

            if (rowNumber > rows.Count - 1)
            {
                throw new IndexOutOfRangeException(string.Format("Requested row index {0} but there are only {1} rows in the table", rowNumber, rows.Count));
            }
                
            RightClickElement(rows[rowNumber]);
        }

        public void RightClickElement(IWebElement element)
        {
            Actions action = new Actions(Driver);
            action.MoveToElement(element);
            action.ContextClick(element).Build().Perform();
        }

        public void ContextMenuOptionIsNotDisplayed(string menuItem)
        {
            Assert.IsFalse(Driver.FindElement(By.CssSelector("li[id$='_ContextMenu_" + menuItem + "']")).Displayed);
        }

        public void ClickIconInRow(int rowNumber, string iconName, string riskTitle = "", string riskId = "null")
        {
            Waiter.Until(d => d.IsInitialDataLoadComplete());
            Waiter.Until(d => !d.IsAjaxRequestInProgress());

            if (riskTitle == "")
            {
                var rows = Element.FindElements(By.CssSelector("tr.grid-row"));
                if (rowNumber > rows.Count - 1)
                    Assert.Fail("Row {0} not found: there are not that many rows in this table.", rowNumber);

                var row = rows[rowNumber];
                var icon = row.FindElement(By.CssSelector("td[id$='_NumberOf" + iconName + "']"));
                icon.Click();
            }else if (riskTitle != "")
            {
                var risks = Element.FindElements(By.CssSelector("tr.grid-row"));
                foreach (var risk in risks)
                    if (risk.FindElement(By.CssSelector("td[id$='_Name']")).Text.Equals(riskTitle) &&
                        riskId == "null" || risk.FindElement(By.CssSelector("td[id$='_CustomRiskRef']")).Text.Equals(riskId))
                    {
                        var icon = risk.FindElement(By.CssSelector("td[id$='_NumberOf" + iconName + "']"));
                        icon.Click();
                    }
            }
            Waiter.Until(d => d.IsInitialDataLoadComplete());
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        public void SetHeaderFilter(string fieldName, string value)
        {

            //filter button
            var filterSelectorString = "td.grid-icon-show-filter-row";

            //Wait until Filter button is visible on page
            Waiter.Until(x => x.FindElement(By.CssSelector(filterSelectorString)).Displayed);

            //If the text box is not visible on the screen already click on the filter button
            if (
                !Driver.FindElement(By.CssSelector(string.Format("input[type='text']#{0}_search_{1}", Id, fieldName)))
                    .Displayed)
            {
                Element.FindElement(By.CssSelector(filterSelectorString)).Click();
            }

            bool searchBoxVisible = false;

            for (var i = 0; i < 30; i++)
            {
                Thread.Sleep(1000);
                var searchBoxes =
                    Driver.FindElements(By.CssSelector(string.Format("input[type='text']#{0}_search_{1}", Id, fieldName)));
                if (searchBoxes.Count > 0 && Driver.FindElement(By.CssSelector(string.Format("input[type='text']#{0}_search_{1}", Id, fieldName))).Displayed)
                {
                    searchBoxVisible = true;
                    break;
                }
            }

            if (!searchBoxVisible)
            {
                Assert.Fail(
                    "Was waiting for the Filter Header Search Text Box to become visible in the Desktop but it DID NOT");
            }

            //filter text box
            var textBox = new WebDriverTextField(Driver, Waiter,
                string.Format("input[type='text']#{0}_search_{1}", Id, fieldName), true);

            textBox.SetValue(value);

            textBox.Enter();

            WaitUntilUiSpinnerIsNotDisplayed();

            // Wait until the Clear Filters And Refresh Icon Appears before continuing
            // In some grids (e.g. the Evaluation Grid, the grid refreshes more than once
            // so we cannot use a standard Wait.Until statement because the element
            // we are waiting for is no longer present after the grid has refreshed
            // so never becomes available therefore we have to check for new ones
            // each time
            Driver.WaitForAjaxToComplete();
            bool clearFiltersAndRefreshIconAppeared = false;

            for (var i = 0; i < 30; i++)
            {
                Thread.Sleep(1000);
                var clearFiltersAndRefreshIcons = Driver.FindElements(By.CssSelector("td.grid-icon-clear-filter"));
                if (clearFiltersAndRefreshIcons.Count > 0)
                {
                    clearFiltersAndRefreshIconAppeared = true;
                    break;
                }

            }

            if (!clearFiltersAndRefreshIconAppeared)
            {
                Assert.Fail(
                    "After searching in the grid, was waiting for the Clear Filters And Refresh Icon to appear, but it DID NOT");
            }

            //Element will now be stale, so reload
            WaitUntilUiSpinnerIsNotDisplayed();
        }
    }
}
