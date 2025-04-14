using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Globalization;
using System.Linq;

namespace FitnessApp.Converters
{
    public class IsPieSeriesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<ISeries> series && series.Any())
            {
                return series.First() is PieSeries<int>;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsNotPieSeriesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<ISeries> series && series.Any())
            {
                return !(series.First() is PieSeries<int>);
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}