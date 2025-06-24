namespace PlcInterfaceApp.Helpers
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class BoolToVisibilityConverter : IValueConverter
    {
        /*        * This converter converts a boolean value to a Visibility value.
         * If the boolean is true, it returns Visibility.Visible; otherwise, it returns Visibility.Collapsed.
         * It can be used in WPF data binding to control the visibility of UI elements based on boolean properties.
         */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is Visibility v && v == Visibility.Visible);
        }
    }
}
