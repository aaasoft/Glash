using Avalonia.Data.Converters;

namespace GlashClientDesktop.Converters
{
    public class ObjectNullToTConverter<T> : IValueConverter
    {
        /// <summary>
        /// 值为null时，对应的返回值
        /// </summary>
        public virtual T NullValue { get; set; }
        /// <summary>
        /// 值为true时，对应的返回值
        /// </summary>
        public virtual T NotNullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return NullValue;
            else
                return NotNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
