using System.Globalization;
using System.Windows.Data;

namespace Delivery.WPF.ValidationRules
{
    public class BoolInverter : IValueConverter
    {
        public static readonly BoolInverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
