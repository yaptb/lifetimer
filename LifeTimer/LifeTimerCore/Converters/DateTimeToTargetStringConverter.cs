using Microsoft.UI.Xaml.Data;
using System;
using LifeTimer.Helpers;

namespace LifeTimer.Converters
{
    public class DateTimeToTargetStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                return DateTimeFormatHelper.FormatTargetDateTime(dateTime);
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("ConvertBack not implemented for DateTimeToTargetStringConverter");
        }
    }
}