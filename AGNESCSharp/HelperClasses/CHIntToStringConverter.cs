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
                return "Variance Reported None Found";
            }
            else if ((byte)value == 1)
            {
                return "$3.00 - $20.00";
            }
            else
            {
                return "$20.01 +";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
