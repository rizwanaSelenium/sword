using System;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using PresentationModel.Model.Desktop;

namespace PresentationModel.Controls
{
    public class DesktopGridTableControl : WebDriverTableControl
    {
        private readonly string _tableId;

        public DesktopGridTableControl(IWebDriver driver, WebDriverWait waiter, string tableId, string recordType) : base(driver, waiter, tableId)
        {
            GridLocators = new DesktopGridLocators(recordType);
            _tableId = tableId;
        }

        public void AssertRecordNameOnRowIs(int rowNumber, string recordName, string recordType)
        {
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));

            if (rowNumber > records.Count)
            {
                throw new WebDriverException("Row number: " +  rowNumber + " was not available in the " + recordType + " grid. Found " + records.Count + " rows.");
            }
            Assert.AreEqual(recordName, records[rowNumber - 1].FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text, "Record found on row: " + rowNumber + " did not have the expected name, on the " + recordType + " grid.");
        }

        public void AssertNumberOfRecordsDisplayed(int numberOfRecordsExpected)
        {
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));
            Assert.AreEqual(numberOfRecordsExpected, records.Count, "Unexpected number of records displayed on the grid. Expected " + numberOfRecordsExpected + " but found " + records.Count);
        }

        public int NumberOfRecordsWithTitle(string recordTitle)
        {
            return Element.FindElements(By.CssSelector(GridLocators.IdColumnLocator)).Where(d => d.Text.Equals(recordTitle)).ToArray().Length;
        }

        public void SelectRecordInGridWithTitle(string recordToSelect, string recordType, bool multiple = false)
        {   
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.TitleColumnLocator)).Where(d => d.Text.Equals(recordToSelect)).ToArray().Length;
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));
            if (recordsCount > 1 && !multiple)
            {
               throw new WebDriverException("More than one record with the title " + recordToSelect + " found on the " + recordType + " grid. Found " + recordsCount + " records. Unable to determin which record to select.");
            } if (recordsCount == 0)
            {
                throw new WebDriverException(recordType + ": " + recordToSelect + " Not Found in " + recordType + " Grid. Unable to select.");
            }

            foreach (var record in records)
                if (record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text.Equals(recordToSelect))
                {
                    if (!record.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Selected)
                    {
                        record.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Click();
                        try
                        {
                            Waiter.Until(d => record.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Selected);
                        }
                        catch (WebDriverTimeoutException ex)
                        {
                            throw new WebDriverTimeoutException("Failed to select record in " + recordType + "grid with title. " + ex);
                        }
                    }
                }
        }

        public void UnselectRecordInGridWithTitle(string recordToUnselect, string recordType, bool multiple = false)
        {
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.TitleColumnLocator)).Where(d => d.Text.Equals(recordToUnselect)).ToArray().Length;
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));
            if (recordsCount > 1 && !multiple)
            {
                throw new WebDriverException("More than one record with the title " + recordToUnselect + " found on the " + recordType + " grid. Found " + recordsCount + " records. Unable to determin which record to unselect.");
            } if (recordsCount == 0)
            {
                throw new WebDriverException(recordType + ": " + recordToUnselect + " not found in " + recordType + " grid. Unable to unselect.");
            }

            foreach (var record in records)
            {
                if (record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text.Equals(recordToUnselect))
                {
                    if (record.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Selected)
                    {
                        record.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Click();
                        try
                        {
                            Waiter.Until(d => !record.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Selected);
                        }
                        catch (WebDriverTimeoutException ex)
                        {
                            throw new WebDriverTimeoutException("Failed to unselect record in " + recordType + "grid with title. " + ex);
                        }
                    }
                }
            }
        }

        public void SelectCopiedRecordInGridWithTitle(string recordTitle, string originalRecordId, string recordType)
        {
            var recordCount = Element.FindElements(By.CssSelector(GridLocators.TitleColumnLocator)).Count(d => d.Text.Equals(recordTitle));
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));
            var originalRecordCount = 0;
            var copiedRecordCount = 0;

            if (recordCount > 2)
            {
                throw new WebDriverException("More than two records with the title " + recordTitle + " found on the " + recordType + " grid. Found " + recordCount + " records. Unable to know which ones are the intended records.");
            }

            foreach (var record in records)
            {
                if (record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text.Equals(recordTitle))
                {
                    if (record.FindElement(By.CssSelector(GridLocators.IdColumnLocator)).Text.Equals(originalRecordId))
                    {
                        originalRecordCount++;
                    }
                    else
                    {
                        copiedRecordCount++;
                        if (!record.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Selected)
                        {
                            record.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Click();
                            try
                            {
                                Waiter.Until(d => record.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Selected);
                            }
                            catch (WebDriverTimeoutException ex)
                            {
                                throw new WebDriverTimeoutException("Failed to select record in " + recordType + "grid with title. " + recordTitle  + ". " + ex);
                            }
                        }
                    }
                }
            }

            if (originalRecordCount > 1 || copiedRecordCount > 1)
            {
                throw new WebDriverException("More records than expected found with the title " + recordTitle + " on the " + recordType + " grid. " + originalRecordCount + " original record(s) found, " + copiedRecordCount + " copy record(s) found. Expected 0 or 1 original records and 1 copy record.");
            }

            if (copiedRecordCount == 0)
            {
                throw new WebDriverException("Copied record: " + recordTitle + " not found, on the " + recordType + " grid.");
            }
        }

        public T OpenRecordInGridWithTitle<T>(string recordTitleToOpen, string recordType) where T : WebDriverArmPage
        {
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.TitleColumnLocator)).Where(d => d.Text.Equals(recordTitleToOpen)).ToArray().Length;
            if (recordsCount > 1)
            {
                throw new WebDriverException("More than one record with the title " + recordTitleToOpen + " found on the " + recordType + " grid. Found " + recordsCount + " records. Unable to determin which record to open.");
            } if (recordsCount == 0)
            {
                throw new WebDriverException("No records with the title " + recordTitleToOpen + " found on the " + recordType + " grid.");
            }

            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));

            foreach (var record in records)
                if (record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text.Equals(recordTitleToOpen))
                {
                    var myWindwos = Driver.WindowHandles.Count;
                    record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator + " a")).Click();
                    try
                    {
                        Waiter.Until(d => Driver.WindowHandles.Count != myWindwos);
                    }
                    catch (WebDriverTimeoutException ex)
                    {
                        throw new WebDriverTimeoutException("Failed to open record " + recordTitleToOpen + " on the " + recordType + " grid." + ex);
                    }
                    break;
                }
            return (T)Activator.CreateInstance(typeof(T), Driver, Waiter, null);
        }

        public T OpenCopiedRecordInGridWithTitle<T>(string recordTitleToOpen, string recordType, string originalRecordId) where T : WebDriverArmPage
        {
            var isFound = false;
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.TitleColumnLocator)).Where(d => d.Text.Equals(recordTitleToOpen)).ToArray().Length;
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));

            if (recordsCount > 2)
            {
                throw new WebDriverException("More than two records found with the title " + recordTitleToOpen + " in the " + recordType + " grid. Expeted to find no more than two.");
            }

            foreach (var record in records)
                if (record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text.Equals(recordTitleToOpen))
                {
                    if (!record.FindElement(By.CssSelector(GridLocators.IdColumnLocator)).Text.Equals(originalRecordId))
                    {
                        record.FindElement(By.CssSelector("td[id$='_Title'] a")).Click();
                        isFound = true;
                        break;
                    }
                }

            if (!isFound)
            {
                throw new WebDriverException("Failed to locate copied record to open. " + recordTitleToOpen + " not found on the " + recordType + " grid.");
            }
            return (T)Activator.CreateInstance(typeof(T), Driver, Waiter, null);
        }

        public string GetIdOfRecordWithTitle(string recordTitle, string recordType)
        {
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.TitleColumnLocator)).Where(d => d.Text.Equals(recordTitle)).ToArray().Length;
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));

            if (recordsCount > 1)
            { throw new WebDriverException("More than one record with the tilte "  + recordTitle + " found on the " + recordType + " grid. Not able to determin which record is intended.");}
            else if (recordsCount == 0)
            { throw new WebDriverException("Record with the title " + recordTitle + " not found on the " + recordType + " grid. Unable to open record.");}

            foreach (var record in records)
                if (record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text.Equals(recordTitle))
                {
                    if (record.FindElement(By.CssSelector(GridLocators.IdColumnLocator)).Text == "")
                    { throw new WebDriverException("Record ID returned a null value. Expected this to be a non null value.");}
                    return record.FindElement(By.CssSelector(GridLocators.IdColumnLocator)).Text;
                }
            throw new WebDriverException("Something went wrong getting record ID, should have exited before this point. Record was not found.");
        }

        public bool AssertRecordPresentWithTitle(string recordTitle, string recordType)
        {
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.TitleColumnLocator)).Where(d => d.Text.Equals(recordTitle)).ToArray().Length;

            if (recordsCount == 0 )
            { Assert.Fail("Record with title " + recordTitle + " not found on the " + recordType + " grid.");}
            if (recordsCount > 1)
            { Assert.Fail("More than one record with title " + recordTitle + " found on the " + recordType + " grid. Unsure if correct record found.");}

            return true;
        }

        public bool AssertRecordNotPresentWithTitle(string recordTitle, string recordType)
        {
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.TitleColumnLocator)).Where(d => d.Text.Equals(recordTitle)).ToArray().Length;

            if (recordsCount > 0)
            { Assert.Fail("Record with title " + recordTitle + " found on the " + recordType + " grid. Expected there to be no records with this name, found " + recordsCount + " records."); }

            return true;
        }

        public bool AssertRecordPresentWithId(string recordId, string recordType)
        {
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.IdColumnLocator)).Where(d => d.Text.Equals(recordId)).ToArray().Length;

            if (recordsCount == 0)
            { Assert.Fail("Record with title " + recordId + " not found on the " + recordType + " grid."); }
            else if (recordsCount > 1)
            { Assert.Fail("More than one record with ID " + recordId + " found on the " + recordType + " grid. Unsure if correct record found."); }

            return true;
        }

        public bool AssertRecordNotPresentWithId(string recordId, string recordType)
        {
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.IdColumnLocator)).Where(d => d.Text.Equals(recordId)).ToArray().Length;

            if (recordsCount > 0)
            { Assert.Fail("Record with ID " + recordId + " found on the " + recordType + " grid. Expected there to be no records with this ID, found " + recordsCount + " records."); }

            return true;
        }

        public T OpenRecordInGridWithId<T>(string recordIdToOpen, string recordType) where T : WebDriverArmPage
        {
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.IdColumnLocator)).Where(d => d.Text.Equals(recordIdToOpen)).ToArray().Length;
            if (recordsCount > 1)
            {
                throw new WebDriverException("More than one record with the ID " + recordIdToOpen + " found on the " + recordType + " grid. Found " + recordsCount + " records. Unable to determin which record to open.");
            }
            if (recordsCount == 0)
            {
                throw new WebDriverException("No records with the ID " + recordIdToOpen + " found on the " + recordType + " grid.");
            }

            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));

            foreach (var record in records)
                if (record.FindElement(By.CssSelector(GridLocators.IdColumnLocator)).Text.Equals(recordIdToOpen))
                {
                    var myWindwos = Driver.WindowHandles.Count;
                    record.FindElement(By.CssSelector(GridLocators.IdColumnLocator + " a")).Click();
                    try
                    {
                        Waiter.Until(d => Driver.WindowHandles.Count != myWindwos);
                    }
                    catch (WebDriverTimeoutException ex)
                    {
                        throw new WebDriverTimeoutException("Failed to open record ID " + recordIdToOpen + " on the " + recordType + " grid." + ex);
                    }
                    break;
                }
            return (T)Activator.CreateInstance(typeof(T), Driver, Waiter, null);
        }

        public void SelectRecordInGridWithId(string recordToSelect, string recordType, bool multiple = false)
        {
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.IdColumnLocator)).Where(d => d.Text.Equals(recordToSelect)).ToArray().Length;
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));
            if (recordsCount > 1 && !multiple)
            {
                throw new WebDriverException("More than one record with the ID " + recordToSelect + " found on the " + recordType + " grid. Found " + recordsCount + " records. Unable to determin which record to select.");
            }
            if (recordsCount == 0)
            {
                throw new WebDriverException(recordType + ": ID " + recordToSelect + " Not Found in " + recordType + " Grid. Unable to select.");
            }

            foreach (var record in records)
                if (record.FindElement(By.CssSelector(GridLocators.IdColumnLocator)).Text.Equals(recordToSelect))
                {
                    if (!record.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Selected)
                    {
                        record.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Click();
                        try
                        {
                            Waiter.Until(d => record.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Selected);
                        }
                        catch (WebDriverTimeoutException ex)
                        {
                            throw new WebDriverTimeoutException("Failed to select record in with ID " + recordToSelect + " in the " + recordType + "grid with title. " + ex);
                        }
                    }
                }
        }

        public void RightClickOnRecordOnGridWithTitle(string recordTitle, string recordType)
        {
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.IdColumnLocator)).Where(d => d.Text.Equals(recordTitle)).ToArray().Length;
            if (recordsCount > 1)
            {
                throw new WebDriverException("More than one record with the ID " + recordTitle + " found on the " + recordType + " grid. Found " + recordsCount + " records. Unable to determin which record to right click on.");
            }
            if (recordsCount == 0)
            {
                throw new WebDriverException("No records with the ID " + recordTitle + " found on the " + recordType + " grid.");
            }

            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));

            foreach (var record in records)
                if (record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text.Equals(recordTitle))
                {
                    var recordToRightClick = record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator));
                    var actions = new Actions(Driver);
                    actions.MoveToElement(recordToRightClick);
                    actions.Build().Perform();
                    actions.Click().Perform();
                    actions.ContextClick().Perform();
                    try
                    {
                        Waiter.Until(d => Driver.FindElement(By.CssSelector(_tableId + "_ContextMenu_Popup")).Displayed);
                    }
                    catch (WebDriverTimeoutException ex)
                    {
                        throw new WebDriverTimeoutException("Timed out waiting for the context menu to be displayed on the " + recordType + " grid." + ex);
                    }
                }
        }

        public void RightClickOnGrid(string recordType, string recordTitle)
        {
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));
            foreach (var record in records)
                if (record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text.Equals(recordTitle))
                {
                    var element = record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator));

                    var action = new Actions(Driver);
                    action.MoveToElement(element);
                    action.ContextClick(element).Build().Perform();
                    try
                    {
                        Waiter.Until(d => Driver.FindElements(By.CssSelector(".contextPopup")).Count(e => e.Displayed) > 0);
                    }
                    catch (WebDriverTimeoutException ex)
                    {
                        throw new WebDriverTimeoutException("Timed out waiting for the grid context menu to be displayed on the " + recordType + " grid. " + ex);
                    }
                }
        }
        
        public void SelectContextMenuItem(string menuItem, string recordType)
        {
            try
            {
                Waiter.Until(d => Driver.FindElements(By.CssSelector("li[id$='_ContextMenu_" + menuItem + "']")).Count >= 1);
                var contextMenus = Driver.FindElements(By.CssSelector("li[id$='_ContextMenu_" + menuItem + "']"));
                foreach (var contextMenu in contextMenus)
                {
                    if (contextMenu.Displayed)
                    {
                        contextMenu.Click();
                        break;
                    }
                }
                Waiter.Until(d => Driver.FindElements(By.CssSelector(".contextPopup")).Count(e => e.Displayed) == 0);
            }
            catch (WebDriverTimeoutException ex)
            { throw new WebDriverTimeoutException("Timeout error when selecting context menu option " + menuItem + ", on the " + recordType + " grid. " + ex);}
        }

        public void DeleteRecordsByTitle(string recordType, string recordTitle, bool multiple = false)
        {
            SelectRecordInGridWithTitle(recordTitle, recordType, multiple);
            RightClickOnGrid(recordType, recordTitle);
            SelectContextMenuItem(GridLocators.DeleteSelectedLocator, recordType);
            try
            {
                Waiter.Until(d => NumberOfRecordsWithTitle(recordTitle) == 0);
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new WebDriverTimeoutException("Timed out while waiting for the record " + recordTitle + " to be deleted. " + ex);
            }
        }

        public void RightClickOnRecordAndSelectMenuItem(string recordType, string recordTitle, string menuItem, bool multiple = false)
        {
            SelectRecordInGridWithTitle(recordTitle, recordType, multiple);
            RightClickOnGrid(recordType, recordTitle);
            SelectContextMenuItem(menuItem, recordType);
        }

        public void OpenRelatedGridByRecordTitle(string currentRecordType, string relatedRecordType, string recordTitle, WebDriverDesktop desktop)
        {
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.TitleColumnLocator)).Where(d => d.Text.Equals(recordTitle)).ToArray().Length;
            if (recordsCount > 1)
            {
                throw new WebDriverException("More than one record with the title " + recordTitle + " found on the " + currentRecordType + " grid. Found " + recordsCount + " records. Unable to determin which record to open.");
            }
            if (recordsCount == 0)
            {
                throw new WebDriverException("No records with the title " + recordTitle + " found on the " + currentRecordType + " grid.");
            }

            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));

            foreach (var record in records)
                if (record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text.Equals(recordTitle))
                {
                    var gridIcon = record.FindElements(By.CssSelector(GridLocators.RelatedGridSelector(relatedRecordType)));
                    if (gridIcon.Count == 0)
                    {
                        throw new WebDriverException("Related record icon not found on grid. Looking for related grid " + relatedRecordType + " on the " + currentRecordType + " grid, for the record " + recordTitle + ".");
                    } if (gridIcon.Count == 1)
                    {
                        gridIcon[0].Click();
                        WaitUntilRelatedGridDisplayed(relatedRecordType, desktop);
                        break;
                    } if (gridIcon.Count > 1)
                    {
                        throw new WebDriverException("Something is wrong found multiple grid icons, found " + gridIcon.Count + " icons for related grid " + relatedRecordType + " on the " + currentRecordType + " grid, for the record " + recordTitle + ".");
                    }
                }
        }

        public void CheckAllRisksInRisksIssuesGrid(int numberOfPages = 1)
        {
            GoToPage(1);

            for (int i = 0 ; i < numberOfPages ; i++)
            {
                var risks = Element.FindElements(By.CssSelector(GridLocators.GridRow));
                foreach (var risk in risks)
                {
                    if (!risk.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Selected)
                    {
                        risk.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Click();
                        try
                        {
                            Waiter.Until(d => risk.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Selected);
                        }
                        catch (WebDriverTimeoutException ex)
                        {
                            throw new WebDriverTimeoutException("Failed to tick records checkbox. Attempting to select all records. " + ex);
                        }
                    }
                }
                NextPage();
            }
        }

        public void UncheckAllRisksInRisksIssuesGrid(int numberOfPages = 1)
        {
            GoToPage(1);

            for (int i = 0 ; i < numberOfPages ; i++)
            {
                var risks = Element.FindElements(By.CssSelector(GridLocators.GridRow));

                foreach (var risk in risks)
                {
                    if (risk.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Selected)
                    {
                        risk.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Click();
                        try
                        {
                            Waiter.Until(d => !risk.FindElement(By.CssSelector(GridLocators.RecordCheckbox)).Selected);
                        }
                        catch (WebDriverTimeoutException ex)
                        {
                            throw new WebDriverTimeoutException("Failed to untick records checkbox. Attempting to unselect all records. " + ex);
                        }
                    }
                }
                NextPage();
            }
        }

        public T DoubleClickOnRecordInGrid<T>(string recordToOpen, string recordType) where T : WebDriverArmPage
        {
            var actions = new Actions(Driver);
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.TitleColumnLocator)).Where(d => d.Text.Equals(recordToOpen)).ToArray().Length;
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));

            if (recordsCount > 1 )
            {
                throw new WebDriverException("More than one record with the title " + recordToOpen + " found on the " + recordType + " grid. Found " + recordsCount + " records. Unable to determin which record to select.");
            }
            if (recordsCount == 0 )
            {
                throw new WebDriverException(recordType + ": " + recordToOpen + " Not Found in " + recordType + " Grid. Unable to select.");
            }

            foreach (var record in records)
            {
                if (record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text.Equals(recordToOpen))
                {
                    var windowsBefore = Driver.WindowHandles.Count;
                    var element = record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator));
                    actions.DoubleClick(element).Perform();

                    try
                    {
                        Waiter.Until(d => Driver.WindowHandles.Count == windowsBefore + 1);
                    }
                    catch (WebDriverTimeoutException ex)
                    {
                        throw new WebDriverTimeoutException("Timed out while waitng for record detail window to open, double clicked on record title. Record to open: " + recordToOpen + ", record Type: " + recordType + ". " + ex);
                    }
                    break;
                }
            }

            return (T)Activator.CreateInstance(typeof(T), Driver, Waiter, null);
        }

        public void ClickOnCellofRecordWithTitle(string recordTitle, string recordType, string cellToClick)
        {
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.TitleColumnLocator)).Where(d => d.Text.Equals(recordTitle)).ToArray().Length;

            if (recordsCount == 0)
            {
                throw new WebDriverException(recordType + ": " + recordTitle + " Not Found in " + recordType + " Grid. Unable to click on requested cell.");
            }
            if (recordsCount > 1)
            {
                throw new WebDriverException("More than one record with the title " + recordTitle + " found on the " + recordType + " grid. Found " + recordsCount + " records. Unable to determin which record requested.");
            }

            foreach (var record in records)
                if (record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text.Equals(recordTitle))
                {
                    var itemToClick = record.FindElements(By.CssSelector(GridLocators.GridCellSelector(cellToClick)));
                    if (itemToClick.Count != 1)
                    {
                        throw new WebDriverException("Cell to click not found or more than one option found, looking for cell " + cellToClick + ". Found " + itemToClick.Count + ".");
                    }
                    itemToClick[0].Click();
                    break;
                }
        }

        public string GetValueOfCellForRecordWithTitle(string recordTitle, string recordType, string cell)
        {
            var records = Element.FindElements(By.CssSelector(GridLocators.GridRow));
            var recordsCount = Element.FindElements(By.CssSelector(GridLocators.TitleColumnLocator)).Where(d => d.Text.Equals(recordTitle)).ToArray().Length;

            if (recordsCount == 0)
            {
                throw new WebDriverException(recordType + ": " + recordTitle + " Not Found in " + recordType + " Grid. Unable to get requested value.");
            }
            if (recordsCount > 1)
            {
                throw new WebDriverException("More than one record with the title " + recordTitle + " found on the " + recordType + " grid. Found " + recordsCount + " records. Unable to determin which record was requested.");
            }

            foreach (var record in records)
                if (record.FindElement(By.CssSelector(GridLocators.TitleColumnLocator)).Text.Equals(recordTitle))
                {
                    var cellToGet = record.FindElements(By.CssSelector(GridLocators.GridCellSelector(cell)));
                    if (cellToGet.Count != 1)
                    {
                        throw new WebDriverException("Cell to get not found or more than one option found, looking for cell " + cell + ". Found " + cellToGet.Count + ".");
                    }
                    return cellToGet[0].Text;
                }
            throw new WebDriverException("Failed to get cell value on the grid, method exited unexpectedly.");
        }

        protected void WaitUntilRelatedGridDisplayed(string relatedGrid, WebDriverDesktop desktop)
        { 
            try
            {
                switch (relatedGrid)
                {
                    case "risk":
                        Waiter.Until(d => desktop.RiskIssuesGrid);
                        break;
                    case "impact":
                        Waiter.Until(d => desktop.ScoringGrid);
                        break;
                    case "plan":
                        Waiter.Until(d => desktop.PlanGrid);
                        break;
                    case "response":
                        Waiter.Until(d => desktop.ResponseGrid);
                        break;
                    case "evaluation":
                        Waiter.Until(d => desktop.EvaluationGrid);
                        break;
                    case "deficiency":
                        Waiter.Until(d => desktop.DeficiencyGrid);
                        break;
                    case "documents":
                        Waiter.Until(d => desktop.DocumentGrid);
                        break;
                    case "audits":
                        Waiter.Until(d => desktop.AuditGrid);
                        break;
                    case "incidents":
                        Waiter.Until(d => desktop.IncidentGrid);
                        break;
                    default:
                        throw new WebDriverException("Unknown grid type unable to wait for grid to be displayed check grid type implemented - " + relatedGrid + ". ");
                }
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new WebDriverTimeoutException("Timed out while waiting for the related grid " + relatedGrid + ", to be displayed. " + ex);
            }
        }
    }
}
