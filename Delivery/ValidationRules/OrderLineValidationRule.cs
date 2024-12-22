using System.Globalization;
using System.Windows.Controls;

namespace Delivery.WPF.ValidationRules
{
    public class OrderLineValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var valueToValidate = value as string;
            return !string.IsNullOrWhiteSpace(valueToValidate) 
                ? ValidationResult.ValidResult 
                : new ValidationResult(false, "TODO: ");
        }

        public bool IsValid { get; set; }
    }
}
