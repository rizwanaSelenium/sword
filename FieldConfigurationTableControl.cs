using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class FieldConfigurationTableControl : WebDriverTableControl
    {
        public FieldConfigurationTableControl(IWebDriver driver, WebDriverWait waiter, string id) : base(driver, waiter, id)
        {
        }

        public void GotoNextPage()
        {
            Waiter.Until(d => d.FindElement(By.CssSelector("img#RV_TC_ST_SF_Table_nextImg"))).Click();
            Waiter.Until(d => !d.IsAjaxRequestInProgress());
        }

        private bool IsBoxChecked(IWebElement element, String attribute)
        {
            bool isBoxChecked = false;
            try
            {
                string value = element.GetAttribute(attribute);
                if (value != null)
                {
                    isBoxChecked = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Caught exception in FieldConfigurationTableControl checking for checkbox status: " + e);
            }
            return isBoxChecked;
        }

        public void MakeFieldVisible(string fieldName)
        {
            var myCheckbox = Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + fieldName + "'] + td[id$='_Visible'] input"));
            if (!IsBoxChecked(myCheckbox, "CHECKED"))
            {
                Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + fieldName + "'] + td[id$='_Visible']")).Click();
            }
        }

        public void MakeFieldNotVisible(string fieldName)
        {
            var myCheckbox = Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + fieldName + "'] + td[id$='_Visible'] input"));
            if (IsBoxChecked(myCheckbox, "CHECKED"))
            {
                Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + fieldName + "'] + td[id$='_Visible']")).Click();
            }
        }

        public void MakeFieldReadOnly(string fieldName)
        {
            var myCheckbox =
                Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + fieldName + "'] + td + td + td[id$='_ReadOnly'] input"));
            if (!IsBoxChecked(myCheckbox, "CHECKED"))
            {
                Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + fieldName + "'] + td + td + td[id$='_ReadOnly']")).Click();
            }
        }

        public void MakeFieldNotReadOnly(string fieldName)
        {
            var myCheckbox =
                Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + fieldName + "'] + td + td + td[id$='_ReadOnly'] input"));
            if (IsBoxChecked(myCheckbox, "CHECKED"))
            {
                Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + fieldName + "'] + td + td + td[id$='_ReadOnly']")).Click();
            }
        }

        public void MakeFieldMandatory(string fieldName)
        {
            var myCheckbox =
                Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + fieldName + "'] + td + td[id$='_Mandatory'] input"));
            if (!IsBoxChecked(myCheckbox, "CHECKED"))
            {
                Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + fieldName + "'] + td + td[id$='_Mandatory']")).Click();
            }
        }

        public void MakeFieldNotMandatory(string fieldName)
        {
            var myCheckbox =
                Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + fieldName + "'] + td + td[id$='_Mandatory'] input"));
            if (IsBoxChecked(myCheckbox, "CHECKED"))
            {
                Driver.FindElement(By.CssSelector(CssSelectorString + " td[title='" + fieldName + "'] + td + td[id$='_Mandatory']")).Click();
            }
        }


        public void MakeAllFieldsVisible()
        {        
            int i = 0;      
            do
            {
                if (i>0)
                {
                    Driver.FindElement(By.Id("RSV_TC_R_RF_Table_nextImg")).Click();
                    WaitForTabPageToBeReady();
                }
                
                var responseGridTable = Driver.FindElement(By.CssSelector("table#RSV_TC_R_RF_Table_content"));
                var myCheckbox = responseGridTable.FindElements(By.CssSelector("td[id$='_Visible'] input"));
                foreach (var fieldCheckbox in myCheckbox)
                {
                    if (!IsBoxChecked(fieldCheckbox, "CHECKED"))
                    {
                        fieldCheckbox.Click();
                    }
                }
                i = i + 1;
            } while (!Driver.FindElement(By.Id("RSV_TC_R_RF_Table_nextImg")).GetAttribute("src").Contains("_disabled"));
        }

        public void MakeAllRiskIssueFieldsVisible()
        {
                int i = 0;
                do
                {
                    if (i > 0)
                    {
                        Driver.FindElement(By.Id("RV_TC_RT_RF_Table_nextImg")).Click();
                        WaitForTabPageToBeReady();
                    }

                    i = i + 1;
                    var riskGridTable = Driver.FindElement(By.XPath(".//div/table[" + i + "][@id='RV_TC_RT_RF_Table_content']"));
                    var myCheckbox = riskGridTable.FindElements(By.CssSelector("td[id$='_Visible'] input"));
                    foreach (var fieldCheckbox in myCheckbox)
                    {

                        if (!IsBoxChecked(fieldCheckbox, "CHECKED"))
                        {
                            fieldCheckbox.Click();
                        }
                    }
                } while (
                    !Driver.FindElement(By.Id("RV_TC_RT_RF_Table_nextImg")).GetAttribute("src").Contains("_disabled"));
        }

        public void StatusValueConfig(string statusValue)
        {
            Driver.FindElement(By.Id("2_Expander")).Click();
            var statusTable = Driver.FindElement(By.Id("RV_Statuses_Table_content"));

            var statusTablerows = statusTable.FindElements(By.CssSelector("tr[class='grid-row']"));
            foreach (var row in statusTablerows)
            {
                string statusname = row.FindElement(By.CssSelector("td[id$='_Description']/span")).Text;
                if (statusname == statusValue)
                {
                    row.FindElement(By.CssSelector("td[id$='_Included']/input")).Click();
                }
            }
        }

        public void AssertScoringFieldNotChecked(string fieldName)
        {
            {
                var pageCount = Driver.FindElement(By.CssSelector("span#RV_TC_ST_SF_Table_pageCountSpan")).Text;
                for (int i = 1; i <= Convert.ToInt16(pageCount); i++)
                {
                    var fieldsTable =
                        Driver.FindElement(By.CssSelector("table#RV_TC_ST_SF_Table_content:nth-child(" + i + ")"));

                    var rows = fieldsTable.FindElements(By.CssSelector("tr.grid-row"));
                    foreach (var item in rows)
                    {
                        var rowFieldName = item.FindElement(
                            By.CssSelector("td[id$='_FieldName']")).Text;
                        if (rowFieldName.Equals(fieldName))
                        {
                            var visibleField =
                                Driver.FindElement(
                                    By.CssSelector("td[title='" + fieldName + "'] + td[id$='_Visible'] input"));
                            Assert.IsTrue(!IsBoxChecked(visibleField, "CHECKED"));
                            i = Convert.ToInt16(pageCount) + 1;
                            break;
                        }
                    }
                    GotoNextPage();
                }
            }
        }

        public void MakeScoringFieldVisible(string fieldName)
        {
            {
                var pageCount = Driver.FindElement(By.CssSelector("span#RV_TC_ST_SF_Table_pageCountSpan")).Text;
                for (int i = 1; i <= Convert.ToInt16(pageCount); i++)
                {
                    var fieldsTable =
                        Driver.FindElement(By.CssSelector("table#RV_TC_ST_SF_Table_content:nth-child(" + i + ")"));

                    var rows = fieldsTable.FindElements(By.CssSelector("tr.grid-row"));
                    foreach (var item in rows)
                    {
                        var rowFieldName = item.FindElement(
                            By.CssSelector("td[id$='_FieldName']")).Text;
                        if (rowFieldName.Equals(fieldName))
                        {
                            var visibleField =
                                Driver.FindElement(
                                    By.CssSelector("td[title='" + fieldName + "'] + td[id$='_Visible'] input"));
                            if (!IsBoxChecked(visibleField, "CHECKED"))
                            {
                                visibleField.Click();
                                i = Convert.ToInt16(pageCount) + 1;
                                break;
                            }
                        }
                    }
                    GotoNextPage();
                }
            }
        }

        public void MakeScoringFieldNotVisible(string fieldName)
        {
            {
                var pageCount = Driver.FindElement(By.CssSelector("span#RV_TC_ST_SF_Table_pageCountSpan")).Text;
                for (int i = 1; i <= Convert.ToInt16(pageCount); i++)
                {
                    var fieldsTable =
                        Driver.FindElement(By.CssSelector("table#RV_TC_ST_SF_Table_content:nth-child(" + i + ")"));

                    var rows = fieldsTable.FindElements(By.CssSelector("tr.grid-row"));
                    foreach (var item in rows)
                    {
                        var rowFieldName = item.FindElement(
                            By.CssSelector("td[id$='_FieldName']")).Text;
                        if (rowFieldName.Equals(fieldName))
                        {
                            var visibleField =
                                Driver.FindElement(
                                    By.CssSelector("td[title='" + fieldName + "'] + td[id$='_Visible'] input"));
                            if (IsBoxChecked(visibleField, "CHECKED"))
                            {
                                visibleField.Click();
                                i = Convert.ToInt16(pageCount) + 1;
                                break;
                            }
                        }
                    }
                    GotoNextPage();
                }
            }
        }
    }
}
