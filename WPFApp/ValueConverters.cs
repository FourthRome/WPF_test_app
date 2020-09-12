using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFApp
{
    [ValueConversion(typeof(int), typeof(String))]
    public class PubNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return System.Convert.ToInt32(value) as object;
            } catch (FormatException e)
            {
                return null;
            }
            
        }
    }
}
