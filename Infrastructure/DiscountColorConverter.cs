using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ShoesStoreApp.Infrastructure
{
    public class DiscountColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && float.TryParse(value.ToString(), out float discount))
            {
                if (discount > 15)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E8B57"));
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
