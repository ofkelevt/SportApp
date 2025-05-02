using System;
using System.IO;
using Microsoft.Maui.Controls;

namespace SportApp.Converters
{
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte[] byteArray && byteArray.Length > 0)
            {
                return ImageSource.FromStream(() => new MemoryStream(byteArray));
            }
            return null; // Return a default image or null if no data is available
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
