using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PresentationModel.Controls
{
    public class WdMvcDropDown : WebDriverArmControl
    {
        private readonly SelectElement _selectElement;

        public WdMvcDropDown(IWebDriver driver, WebDriverWait waiter, string fieldsetId, string selectId)
            : base(driver, waiter, null)
        {
            if (fieldsetId != "")
            {
                SetSelectorString("fieldset#" + fieldsetId + " div.formrow div select#" + selectId);
            }
            else
            {
                SetSelectorString("fieldset div.formrow div select#" + selectId);
            }

            var elementParent = Element.FindElement(By.XPath(".." + "/" + ".."));
            LabelElement = elementParent.FindElement(By.CssSelector(" label"));
            _selectElement = new SelectElement(Element);
        }

        public void SetValue(string value)
        {
            WaitForElementToAppear();

            Assert.True(_selectElement.Options.Select(o => o.Text).Contains(value));
            _selectElement.SelectByText(value);

            Waiter.Until(d => GetValue() == value);
        }

        public string GetValue()
        {
            WaitForElementToAppear();
            return _selectElement.SelectedOption.Text;
        }

        public void AssertEquals(string text)
        {
            Assert.AreEqual(text, _selectElement.SelectedOption.Text);
        }

        public void AssertOptionCountEquals(int count)
        {
            Assert.AreEqual(count, _selectElement.Options.Count);
        }

        public void AssertContainsOptions(params string[] options)
        {
            var listOptions = _selectElement.Options.Select(o => o.Text).ToList();

            foreach (var option in options)
                Assert.True(listOptions.Contains(option), "Dropdown does not contain option '" + option + "'");
        }

        public override void AssertEnabled()
        {
            Assert.True(Element.Enabled);
        }

        public override void AssertDisabled()
        {
            var disabledElement = Driver.FindElement(By.CssSelector(CssSelectorString + " div.disabled-dropdown"));

            Assert.False(Element.Displayed);
            Assert.True(disabledElement.Displayed);
        }
    }
}
