﻿using Windows.ApplicationModel.Resources;

namespace ISynergy.Framework.UI.Extensions
{
    /// <summary>
    /// Class ResourceExtensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// The resource loader
        /// </summary>
        private static readonly ResourceLoader ResLoader = new ResourceLoader();

        /// <summary>
        /// Gets the localized.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns>System.String.</returns>
        public static string GetLocalized(this string resourceKey)
        {
            return ResLoader.GetString(resourceKey);
        }
    }
}