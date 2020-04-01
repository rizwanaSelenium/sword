using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WebDriverDropDownTextBoxEditText : WebDriverArmControl
    {
        private int _cloneNumber;
        private readonly string _selector;
        public WebDriverDropDownTextBoxEditText(IWebDriver driver, WebDriverWait waiter, string selector)
            : base(driver, waiter, null)
        {
         
            _selector = selector;
            XPathString = "//div[@class='"+ _selector + "' and contains(.,'Please select...')]";
        }

        public void SelectValue(string option)
        {
            Element.Click();
            Element.SendKeys(option);
            Element.SendKeys(Keys.Enter);
            XPathString = "//div[@class='" + _selector + "' and contains(lower-case(.),'" + option + "')]";
        }

        public void ChooseDropdownItem(string fieldName, string fieldValue)
        {
            var control = new WebDriverDropDownTextBoxEditText(Driver, Waiter, "dropDownTextBox editText");
            control.SelectValue(fieldName.ToLower());

            var dropDown = new WebDriverDropDown(Driver, Waiter, "div.armcontrol#clone" + _cloneNumber + " select", true);
            _cloneNumber++;

            dropDown.SetValue(fieldValue);
        }

        public void ChooseTextFieldItem(string fieldName, string fieldValue)
        {
            var control = new WebDriverDropDownTextBoxEditText(Driver, Waiter, "dropDownTextBox editText");
            control.SelectValue(fieldName.ToLower());

            var textField = new WebDriverTextField(Driver, Waiter, "div.armcontrol#clone" + _cloneNumber + " input[type='text']", true);
            _cloneNumber++;

            textField.SetValue(fieldValue);
        }

        public void ChooseTomorrow(string fieldName)
        {
            var control = new WebDriverDropDownTextBoxEditText(Driver, Waiter, "dropDownTextBox editText");
            control.SelectValue(fieldName.ToLower());

            var dateField = new WebDriverDatePicker(Driver, Waiter, "div.armcontrol#clone" + _cloneNumber, true);
            _cloneNumber++;

            dateField.SetTomorrow();
        }

    }

}
