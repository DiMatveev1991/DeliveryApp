using System;
using System.Globalization;
using System.Windows.Controls;

namespace Delivery.WPF.ValidationRules
{
	internal class DoubleValidationRules : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value == null) return new ValidationResult(false, "Значение не должно быть пустым.");

			// Приведение к строке
			var strValue = value as string ?? value?.ToString();

			if (string.IsNullOrWhiteSpace(strValue)) return new ValidationResult(false, "Значение не должно быть пустым.");

			// Попытка парсинга числа с учетом текущей культуры
			if (!double.TryParse(strValue, NumberStyles.Any, cultureInfo, out var doubleValue))
			{
				return new ValidationResult(false, "Неверный формат числа.");
			}

			if (doubleValue <= 0)
			{
				return new ValidationResult(false, "Число должно быть больше нуля.");
			}

			return new ValidationResult(true, null);
		}
	}
}
