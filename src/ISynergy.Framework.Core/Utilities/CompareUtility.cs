using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Framework.Core.Utilities;

/// <summary>
/// Class CompareUtility.
/// </summary>
public static class CompareUtility
{
    /// <summary>
    /// Compares the object.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="destination">The destination.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    public static List<string> CompareObject(object source, object destination)
    {
        var oType = source.GetType();

        return (from oProperty in oType.GetProperties().EnsureNotNull() let oOldValue = oProperty.GetValue(source, null) let oNewValue = oProperty.GetValue(destination, null) where !Equals(oOldValue, oNewValue) let sOldValue = oOldValue is null ? "null" : oOldValue.ToString() let sNewValue = oNewValue is null ? "null" : oNewValue.ToString() select "Property " + oProperty.Name + " was: " + sOldValue + "; is: " + sNewValue).ToList();
    }

    /// <summary>
    /// Compares the specified operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="operation">The operation.</param>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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

    /// <summary>
    /// Compares the specified operation.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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
