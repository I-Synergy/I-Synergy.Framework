using System;
using Windows.UI.Xaml.Resources;

namespace ISynergy.Framework.Windows.Controls
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
        /// The light resources path
        /// </summary>
        private static string lightResourcesPath;
        /// <summary>
        /// The dark resources path
        /// </summary>
        private static string darkResourcesPath;
        /// <summary>
        /// The high contrast resources path
        /// </summary>
        private static string highContrastResourcesPath;

        /// <summary>
        /// Initializes static members of the <see cref="UserThemeResources" /> class.
        /// </summary>
        static UserThemeResources()
        {
            EnsureCustomXamlResourceLoader();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserThemeResources" /> class.
        /// </summary>
        public UserThemeResources()
        {
            EnsureCustomXamlResourceLoader();
        }

        /// <summary>
        /// Gets or sets the <see cref="Uri" /> path to the resource dictionary containing theme resource definitions for the Dark theme.
        /// </summary>
        /// <value>The dark resources path.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string DarkResourcesPath
        {
            get
            {
                return darkResourcesPath;
            }
            set
            {
                darkResourcesPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Uri" /> path to the resource dictionary containing theme resource definitions for the Light theme.
        /// </summary>
        /// <value>The light resources path.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string LightResourcesPath
        {
            get
            {
                return lightResourcesPath;
            }
            set
            {
                lightResourcesPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Uri" /> path to the resource dictionary containing theme resource definitions for the HighContrast theme.
        /// </summary>
        /// <value>The high contrast resources path.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string HighContrastResourcesPath
        {
            get
            {
                return highContrastResourcesPath;
            }
            set
            {
                highContrastResourcesPath = value;
            }
        }

        /// <summary>
        /// Gets the URI by path.
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        /// <returns>Uri.</returns>
        internal static Uri GetUriByPath(string resourceId)
        {
            if (resourceId == LightResourcesPathKeyName)
            {
                if (!string.IsNullOrEmpty(lightResourcesPath))
                {
                    return new Uri(lightResourcesPath);
                }
            }
            else if (resourceId == DarkResourcesPathKeyName)
            {
                if (!string.IsNullOrEmpty(darkResourcesPath))
                {
                    return new Uri(darkResourcesPath);
                }
            }
            else if (resourceId == HighContrastResourcesPathKeyName)
            {
                if (!string.IsNullOrEmpty(highContrastResourcesPath))
                {
                    return new Uri(highContrastResourcesPath);
                }
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
