using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using CarWashFinancialSystem.Models;

namespace CarWashFinancialSystem.Converters
{
    public class RoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserRole role)
            {
                return role == UserRole.Administrator ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}