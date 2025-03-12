using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace FitnessApp.Convertor;

public class BooleanToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isUser)
            return isUser ? Brush.Parse("#d9d9d9") : Brush.Parse("#3f85b7");
        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}