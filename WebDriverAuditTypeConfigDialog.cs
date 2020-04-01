using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverAuditTypeConfigDialog : WebDriverArmControl
    {
        public WebDriverDropDown Record;
        public WebDriverDropDown Configuration;

        public WebDriverDropDownTextBoxFilterItem SelectFieldCriteriaDropDown;
        public WebDriverDropDownTextBoxOperatorItem Operator;
        public WebDriverButton AddCriteriaButton;

        public readonly FieldConfigurationTableControl Table;

        public WebDriverButton OkButton;
        public WebDriverButton SaveButton;
        public WebDriverButton NewButton;
        public WebDriverButton DeleteButton;
        public WebDriverButton CancelButton;
        public WebDriverButton HelpButton;

        public WebDriverAuditTypeConfigDialog(IWebDriver driver, WebDriverWait waiter, string id) : base(driver, waiter, "div#" + id)
        {
            Record = new WebDriverDropDown(driver, waiter, "ACV_Record");
            Configuration = new WebDriverDropDown(driver, waiter, "ACV_Configuration");

            SelectFieldCriteriaDropDown = new WebDriverDropDownTextBoxFilterItem(driver, waiter, "property");
            Operator = new WebDriverDropDownTextBoxOperatorItem(driver, waiter, "operator");
            AddCriteriaButton = new WebDriverButton(driver, waiter, "ACV_FieldCriteria_AddCriteria_btn");

            Table = new FieldConfigurationTableControl(driver, waiter, id);

            OkButton = new WebDriverButton(driver, waiter, "ACV_OK_btn");
            SaveButton = new WebDriverButton(driver, waiter, "ACV_Save_btn");
            NewButton = new WebDriverButton(driver, waiter, "ACV_New_btn");
            DeleteButton = new WebDriverButton(driver, waiter, "ACV_Delete_btn");
            CancelButton = new WebDriverButton(driver, waiter, "ACV_Cancel_btn");
            HelpButton = new WebDriverButton(driver, waiter, "ACV_Help_btn");
        }

        public void AddCriteria()
        {
            AddCriteriaButton.AssertEnabled();
            AddCriteriaButton.Click();

        }

        public void EnableField(string fieldName)
        {
            Table.MakeFieldVisible(fieldName);

        }

        public void DisableField(string fieldName)
        {
            Table.MakeFieldNotVisible(fieldName);

        }

        public void SetReadOnlyField(string fieldName)
        {
            Table.MakeFieldReadOnly(fieldName);
        }

        public void SetNotReadOnlyField(string fieldname)
        {
            Table.MakeFieldNotReadOnly(fieldname);
        }

        public void SetMandatoryField(string fieldName)
        {
            Table.MakeFieldMandatory(fieldName);
        }

        public void SetNotMandatoryField(string fieldName)
        {
            Table.MakeFieldNotMandatory(fieldName);
        }

        public void SaveAndClose(bool expectPopup)
        {
            if (!Driver.Title.StartsWith("*") && !Driver.Title.StartsWith(" *")) return;
            OkButton.Click();
            if (expectPopup)
            {
                Driver.FindElement(By.CssSelector("div#UIPrompt")).FindElement(By.CssSelector("button[title='Yes']")).Click();
            }
        }

        public void Delete()
        {
            DeleteButton.AssertEnabled();
            DeleteButton.Click();
            Waiter.Until(d => d.FindElement(By.CssSelector("div#UIPrompt")));
            Driver.FindElement(By.CssSelector("div#UIPrompt")).FindElement(By.CssSelector("button[title='Yes']")).Click();
        }

        public void Save()
        {
            if (Driver.Title.StartsWith("*") || Driver.Title.StartsWith(" *"))
            {
                SaveButton.AssertEnabled();
                SaveButton.Click();
                Waiter.Until(d => !d.IsAjaxRequestInProgress());
            }
            else
            {
                Console.WriteLine("No Change Detected, Save Button Not Enabled");
            }
        }

        public WebDriverNewAuditTypeConfigDialog New()
        {
            NewButton.AssertEnabled();
            NewButton.Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
            return new WebDriverNewAuditTypeConfigDialog(Driver, Waiter, "ACV_Table");
        }
    }

    public class WebDriverNewAuditTypeConfigDialog : WebDriverArmControl
    {
        public WebDriverDropDown Record;
        public WebDriverTextField Configuration;

        public readonly FieldConfigurationTableControl Table;

        public WebDriverButton OkButton;
        public WebDriverButton SaveButton;
        public WebDriverButton NewButton;
        public WebDriverButton DeleteButton;
        public WebDriverButton CancelButton;
        public WebDriverButton HelpButton;

        public WebDriverNewAuditTypeConfigDialog(IWebDriver driver, WebDriverWait waiter, string id)
            : base(driver, waiter, "div#" + id)
        {
            Record = new WebDriverDropDown(driver, waiter, "ACV_Record");
            Configuration = new WebDriverTextField(driver, waiter, "ACV_Configuration");

            Table = new FieldConfigurationTableControl(driver, waiter, id);

            OkButton = new WebDriverButton(driver, waiter, "ACV_OK_btn");
            SaveButton = new WebDriverButton(driver, waiter, "ACV_Save_btn");
            NewButton = new WebDriverButton(driver, waiter, "ACV_New_btn");
            DeleteButton = new WebDriverButton(driver, waiter, "ACV_Delete_btn");
            CancelButton = new WebDriverButton(driver, waiter, "ACV_Cancel_btn");
            HelpButton = new WebDriverButton(driver, waiter, "ACV_Help_btn");
        }

        public WebDriverAuditTypeConfigDialog Save()
        {
            if (Driver.Title.StartsWith("*") || Driver.Title.StartsWith(" *"))
            {
                SaveButton.AssertEnabled();
                SaveButton.Click();
                Waiter.Until(d => !d.IsAjaxRequestInProgress());
                return new WebDriverAuditTypeConfigDialog(Driver, Waiter, "ACV_Table");
            }
            else
            {
                Console.WriteLine("No Change Detected, Save Button Not Enabled");
            }
            return null;
        }
    }
}
