using System;
using System.Collections.Generic;
using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Framework.Core.Utilities
{
    public static class CompareUtility
    {
        public static List<string> CompareObject(object source, object destination)
        {
            var result = new List<string>();

            var oType = source.GetType();

            foreach (var oProperty in oType.GetProperties().EnsureNotNull())
            {
                var oOldValue = oProperty.GetValue(source, null);
                var oNewValue = oProperty.GetValue(destination, null);

                // this will handle the scenario where either value is null
                if (!Equals(oOldValue, oNewValue))
                {
                    // Handle the display values when the underlying value is null
                    var sOldValue = oOldValue is null ? "null" : oOldValue.ToString();
                    var sNewValue = oNewValue is null ? "null" : oNewValue.ToString();

                    result.Add("Property " + oProperty.Name + " was: " + sOldValue + "; is: " + sNewValue);
                }
            }

            return result;
        }

        public static bool Compare<T>(string operation, T value1, T value2) where T : IComparable
        {
            return operation switch
            {
                "==" => value1.CompareTo(value2) == 0,
                "!=" => value1.CompareTo(value2) != 0,
                ">" => value1.CompareTo(value2) > 0,
                ">=" => value1.CompareTo(value2) >= 0,
                "<" => value1.CompareTo(value2) < 0,
                "<=" => value1.CompareTo(value2) <= 0,
                _ => false,
            };
        }

        public static bool Compare(string operation, object value1, object value2)
        {
            return operation switch
            {
                "==" => value1 == value2,
                "!=" => value1 != value2,
                _ => false,
            };
        }
    }
}
