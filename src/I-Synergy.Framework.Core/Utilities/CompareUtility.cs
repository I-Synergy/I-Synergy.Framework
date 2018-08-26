using ISynergy.Extensions;
using System;
using System.Collections.Generic;

namespace ISynergy.Utilities
{
    public static class CompareUtility
    {
        public static List<string> CompareObject(object source, object destination)
        {
            List<string> result = new List<string>();

            var oType = source.GetType();

            foreach (var oProperty in oType.GetProperties().EnsureNotNull())
            {
                var oOldValue = oProperty.GetValue(source, null);
                var oNewValue = oProperty.GetValue(destination, null);
                
                // this will handle the scenario where either value is null
                if (!object.Equals(oOldValue, oNewValue))
                {
                    // Handle the display values when the underlying value is null
                    var sOldValue = oOldValue == null ? "null" : oOldValue.ToString();
                    var sNewValue = oNewValue == null ? "null" : oNewValue.ToString();

                    result.Add("Property " + oProperty.Name + " was: " + sOldValue + "; is: " + sNewValue);
                }
            }

            return result;
        }

        public static bool Compare<T>(string operation, T value1, T value2) where T:IComparable
        {
            switch (operation)
            {
                case "==": return value1.CompareTo(value2) == 0;
                case "!=": return value1.CompareTo(value2) != 0;
                case ">": return value1.CompareTo(value2) > 0;
                case ">=": return value1.CompareTo(value2) >= 0;
                case "<": return value1.CompareTo(value2) < 0;
                case "<=": return value1.CompareTo(value2) <= 0;
                default: return false;
            }
        }
    }
}
