using System;

#if (WINDOWS_UWP || HAS_UNO)
using Windows.UI.Xaml.Resources;
#else
using Microsoft.UI.Xaml.Resources;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Defines an entry point allowing user-specified resources to replace the built-in theme resources.
    /// </summary>
    public sealed class UserThemeResources
    {
        /// <summary>
        /// The dark resources path key name
        /// </summary>
        internal const string DarkResourcesPathKeyName = "DarkResourcesPath";
        /// <summary>
        /// The light resources path key name
        /// </summary>
        internal const string LightResourcesPathKeyName = "LightResourcesPath";
        /// <summary>
        /// The high contrast resources path key name
        /// </summary>
        internal const string HighContrastResourcesPathKeyName = "HighContrastResourcesPath";

        /// <summary>
        /// Initializes static members of the <see cref="UserThemeResources" /> class.
        /// </summary>
        static UserThemeResources()
        {
            EnsureCustomXamlResourceLoader();
        }

        /// <summary>
        /// Gets or sets the <see cref="Uri" /> path to the resource dictionary containing theme resource definitions for the Dark theme.
        /// </summary>
        /// <value>The dark resources path.</value>
        public static string DarkResourcesPath { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Uri" /> path to the resource dictionary containing theme resource definitions for the Light theme.
        /// </summary>
        /// <value>The light resources path.</value>
        public static string LightResourcesPath { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Uri" /> path to the resource dictionary containing theme resource definitions for the HighContrast theme.
        /// </summary>
        /// <value>The high contrast resources path.</value>
        public static string HighContrastResourcesPath { get; set; }

        /// <summary>
        /// Gets the URI by path.
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        /// <returns>Uri.</returns>
        internal static Uri GetUriByPath(string resourceId)
        {
            if (resourceId == LightResourcesPathKeyName && !string.IsNullOrEmpty(LightResourcesPath))
            {
                return new Uri(LightResourcesPath);
            }
            else if (resourceId == DarkResourcesPathKeyName && !string.IsNullOrEmpty(DarkResourcesPath))
            {
                return new Uri(DarkResourcesPath);
            }
            else if (resourceId == HighContrastResourcesPathKeyName && !string.IsNullOrEmpty(HighContrastResourcesPath))
            {
                return new Uri(HighContrastResourcesPath);
            }

            return null;
        }

        /// <summary>
        /// Ensures the custom xaml resource loader.
        /// </summary>
        private static void EnsureCustomXamlResourceLoader()
        {
            if (CustomXamlResourceLoader.Current is null)
            {
                CustomXamlResourceLoader.Current = new UserThemeResourceLoader();
            }
        }
    }
}
