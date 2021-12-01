﻿using System;
using System.Globalization;

namespace ISynergy.Framework.Synchronization.Core
{
    public static class SyncGlobalization
    {
        /// <summary>
        /// Gets or Sets the string comparison used when comparing string from data source.
        /// Default is Invariant Ignore Case
        /// </summary>
        public static StringComparison DataSourceStringComparison { get; set; }
            = StringComparison.InvariantCultureIgnoreCase;

        /// <summary>
        /// Gets or Sets the number decimal separator used to parse decimal float and double from data source.
        /// Default is Invariant Number Decimal Separator (".")
        /// </summary>

        public static string DataSourceNumberDecimalSeparator { get; set; }
            = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;


        /// <summary>
        /// Gets a boolean indicating if the StringComparison is case sensitive
        /// </summary>
        /// <returns></returns>
        public static bool IsCaseSensitive()
        {
            return DataSourceStringComparison.HasFlag(
                StringComparison.InvariantCulture | StringComparison.CurrentCulture | StringComparison.Ordinal);

        }

        static SyncGlobalization()
        {
        }

    }
}
