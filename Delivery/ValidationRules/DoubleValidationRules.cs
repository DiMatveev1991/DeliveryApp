using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Delivery.WPF.ValidationRules
{
	internal class DoubleValidationRules : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			 double doubleValue = 0;

			try
			{
				if (((string)value).Length > 0)
					doubleValue = Int32.Parse((String)value);
			}
			catch (Exception e)
			{
				return new ValidationResult(false, $"Illegal characters or {e.Message}");
			}

			if (doubleValue <= 0)
			{
				return new ValidationResult(false,
					"Не может быть меньше нуля");
			}
			return new ValidationResult(true, null);
		}
	}
}
