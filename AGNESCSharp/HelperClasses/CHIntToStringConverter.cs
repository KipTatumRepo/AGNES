using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AGNESCSharp
{
    public class CHIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if ((byte)value == 0)
            {
                return "No Value";
            }
            else if ((byte)value == 1)
            {
                return "Half";
            }
            else
            {
                return "Full";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
