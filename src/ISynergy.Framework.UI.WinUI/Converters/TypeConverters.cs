﻿using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using System.Globalization;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// A helper class that enables converting values specified in markup (strings) to their object representation.
/// </summary>
internal static class TypeConverters
{
    private const string ContentControlFormatString = "<ContentControl xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:c='using:{0}'><c:{1}>{2}</c:{1}></ContentControl>";

    /// <summary>
    /// Converts string representation of a value to its object representation.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="destinationTypeFullName">The full name of the destination type.</param>
    /// <returns>Object representation of the string value.</returns>
    /// <exception cref="ArgumentNullException">destinationTypeFullName cannot be null.</exception>
    public static Object? Convert(string value, string destinationTypeFullName)
    {
        if (string.IsNullOrEmpty(destinationTypeFullName))
        {
            throw new ArgumentNullException(nameof(destinationTypeFullName));
        }

        string scope = TypeConverters.GetScope(destinationTypeFullName);

        // Value types in the "System" namespace must be special cased due to a bug in the xaml compiler
        if (string.Equals(scope, "System", StringComparison.Ordinal))
        {
            if (string.Equals(destinationTypeFullName, (typeof(string).FullName), StringComparison.Ordinal))
            {
                return value;
            }

            if (string.Equals(destinationTypeFullName, typeof(bool).FullName, StringComparison.Ordinal))
            {
                return bool.Parse(value);
            }
            if (string.Equals(destinationTypeFullName, typeof(int).FullName, StringComparison.Ordinal))
            {
                return int.Parse(value, CultureInfo.InvariantCulture);
            }
            if (string.Equals(destinationTypeFullName, typeof(double).FullName, StringComparison.Ordinal))
            {
                return double.Parse(value, CultureInfo.InvariantCulture);
            }
        }

        string type = TypeConverters.GetType(destinationTypeFullName);
        string contentControlXaml = string.Format(CultureInfo.InvariantCulture, TypeConverters.ContentControlFormatString, scope, type, value);

        var contentControl = XamlReader.Load(contentControlXaml) as ContentControl;

        if (contentControl is not null)
        {
            return contentControl.Content;
        }

        return null;
    }

    private static String GetScope(string name)
    {
        var indexOfLastPeriod = name.LastIndexOf('.');
        if (indexOfLastPeriod != name.Length - 1)
            return name.Substring(0, indexOfLastPeriod);

        return name;
    }

    private static String GetType(string name)
    {
        var indexOfLastPeriod = name.LastIndexOf('.');
        if (indexOfLastPeriod != name.Length - 1)
            return name.Substring(indexOfLastPeriod + 1);

        return name;
    }
}
