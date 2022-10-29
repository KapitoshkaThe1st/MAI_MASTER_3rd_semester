using System;
using System.Globalization;
using System.Numerics;
using System.Windows.Data;

namespace RSA_Demo
{
    public class BigIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((BigInteger)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BigInteger.Parse(value as string);
        }
    }
}
