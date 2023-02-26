﻿using System;
using System.Collections.ObjectModel;
using ISynergy.Framework.Core.Collections;
using ISynergy.Framework.Mvvm.Abstractions;
using Microsoft.UI.Xaml.Data;

namespace ISynergy.Framework.UI.Converters
{
    /// <summary>
    /// Class BladeVisibilityConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class BladeVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableConcurrentCollection<IView>)
            {
                if (value is ObservableConcurrentCollection<IView> blades && blades.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
