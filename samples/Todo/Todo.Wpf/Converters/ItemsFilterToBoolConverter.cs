using System;
using System.Globalization;
using System.Windows.Data;
using Todo.Engine.Core;

namespace Todo.Wpf.Converters
{
    public class ItemsFilterToBoolConverter : IValueConverter
    {
        public ItemsFilter TrueValue { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var filter = (ItemsFilter)value;
            return filter == TrueValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
