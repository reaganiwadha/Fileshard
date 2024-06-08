using System.Globalization;
using System.Windows.Data;

namespace Fileshard.Frontend.Helpers
{
    public class DimentionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
              object parameter, CultureInfo culture)
        {
            return (double)value / double.Parse((string)parameter);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
