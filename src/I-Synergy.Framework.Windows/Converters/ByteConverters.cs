using ISynergy.Helpers;
using System;
using System.IO;
using Windows.UI.Xaml.Data;

namespace ISynergy.Converters
{
    public class BytesToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value != null)
            {
                return ImageHelper.ConvertByteArray2ImageSourceAsync((byte[])value);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return new NotImplementedException();
        }
    }

    public class ByteArrayToStreamConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Stream result = null;

            if (value is byte[] && value != null)
            {
                byte[] bytes = value as byte[];

                result = new MemoryStream();
                result.Write(bytes, 0, bytes.Length);
                result.Position = 0;
            }
            
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
