using System;
using System.Globalization;
using System.Windows.Data;

namespace cbhk_environment.Generators.DataPackGenerator.DatapackInitializationForms
{
    public class HomePageHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double height)
            {
                return height - 130;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
