using GalaSoft.MvvmLight.Ioc;
using ISynergy.Extensions;
using ISynergy.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ISynergy.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public Type EnumType { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                if (!Enum.IsDefined(EnumType, value))
                {
                    throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum".GetLocalized());
                }

                var enumValue = Enum.Parse(EnumType, enumString);

                return enumValue.Equals(value);
            }

            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                return Enum.Parse(EnumType, enumString);
            }

            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized());
        }
    }

    public class EnumToArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var list = new List<KeyValuePair<int, string>>();
            var enumValues = Enum.GetValues(value.GetType());

            foreach (Enum item in enumValues)
            {
                list.Add(new KeyValuePair<int, string>(System.Convert.ToInt32(item), GetDescription(item)));
            }

            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static string GetDescription(Enum value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var description = value.ToString();
            var fieldInfo = value.GetType().GetField(description);
            var attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0)
                description = SimpleIoc.Default.GetInstance<ILanguageService>().GetString(attributes[0].Description);

            return description;
        }
    }

    public class EnumToResourceImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                switch (value.ToString())
                {
                    case "Phone":
                        return Application.Current.Resources["tile_phone"];

                    case "Mobile":
                        return Application.Current.Resources["tile_mobile"];

                    case "Fax":
                        return Application.Current.Resources["tile_fax"];

                    case "Email":
                        return Application.Current.Resources["tile_email"];

                    case "URL":
                        return Application.Current.Resources["tile_internet"];

                    default:
                        return "";
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return GetDescription(Enum.Parse(value.GetType(), value.ToString()) as Enum);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static string GetDescription(Enum value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var description = value.ToString();
            var fieldInfo = value.GetType().GetField(description);
            var attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                description = SimpleIoc.Default.GetInstance<ILanguageService>().GetString(attributes[0].Description);
            }

            return description;
        }
    }
}
