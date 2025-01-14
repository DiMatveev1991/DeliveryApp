using Delivery.DTOs;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Delivery.WPF.ValidationRules
{
	public class StringValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value is string textValue)
			{
				if (string.IsNullOrWhiteSpace(textValue))
				{
					return new ValidationResult(false, "Item name cannot be empty.");
				}
				return new ValidationResult(true, null);
			}
			return new ValidationResult(false, "Не удается преобразовать в строку");
		}
	}
}
	





