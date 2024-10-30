using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace OPCua_APP.Converters
{
    public class boolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           var state = (bool)value;
            SolidColorBrush color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#22B14C"));
            if (state) return color;
            else return Brushes.Brown;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
