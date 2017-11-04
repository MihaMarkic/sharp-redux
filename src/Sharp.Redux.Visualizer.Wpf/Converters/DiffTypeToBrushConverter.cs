using Sharp.Redux.Visualizer.Core;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Sharp.Redux.Visualizer.Wpf.Converters
{
    public class DiffTypeToBrushConverter : IValueConverter
    {
        static readonly SolidColorBrush added = new SolidColorBrush(Colors.DarkBlue);
        static readonly SolidColorBrush modified = new SolidColorBrush(Colors.DarkGreen);
        static readonly SolidColorBrush removed = new SolidColorBrush(Colors.DarkOrange);
        static readonly SolidColorBrush defaultBrush = new SolidColorBrush(Colors.Black);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DiffType)
            {
                switch ((DiffType)value)
                {
                    case DiffType.Added:
                        return added;
                    case DiffType.Modified:
                        return modified;
                    case DiffType.Removed:
                        return removed;
                    default:
                        return defaultBrush;
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
