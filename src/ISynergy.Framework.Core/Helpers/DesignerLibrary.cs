using System;

namespace ISynergy.Framework.Core.Helpers
{
    /// <summary>
    /// Helper class for platform detection.
    /// </summary>
    internal static class DesignerLibrary
    {
        /// <summary>
        /// Gets the detected designer library.
        /// </summary>
        /// <value>The detected designer library.</value>
        internal static DesignerPlatformLibrary DetectedDesignerLibrary
        {
            get
            {
                if (_detectedDesignerPlatformLibrary is null)
                {
                    _detectedDesignerPlatformLibrary = GetCurrentPlatform();
                }
                return _detectedDesignerPlatformLibrary.Value;
            }
        }

        /// <summary>
        /// Gets the current platform.
        /// </summary>
        /// <returns>DesignerPlatformLibrary.</returns>
        private static DesignerPlatformLibrary GetCurrentPlatform()
        {
            // We check Silverlight first because when in the VS designer, the .NET libraries will resolve
            // If we can resolve the SL libs, then we're in SL or WP
            // Then we check .NET because .NET will load the WinRT library (even though it can't really run it)
            // When running in WinRT, it will not load the PresentationFramework lib

            // Check .NET 
            var cmdm = Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            if (cmdm is not null) // loaded the assembly, could be .net 
            {
                return DesignerPlatformLibrary.Net;
            }

            // check WinRT next
            var wadm = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime");
            if (wadm is not null)
            {
                return DesignerPlatformLibrary.WinRt;
            }

            return DesignerPlatformLibrary.Unknown;
        }


        /// <summary>
        /// The detected designer platform library
        /// </summary>
        private static DesignerPlatformLibrary? _detectedDesignerPlatformLibrary;

        /// <summary>
        /// The is in design mode
        /// </summary>
        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets a value indicating whether this instance is in design mode.
        /// </summary>
        /// <value><c>true</c> if this instance is in design mode; otherwise, <c>false</c>.</value>
        public static bool IsInDesignMode
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
#if PORTABLE
                    _isInDesignMode = IsInDesignModePortable();
#elif SILVERLIGHT
                    _isInDesignMode = System.ComponentModel.DesignerProperties.IsInDesignTool;
#elif __UWP__
                    _isInDesignMode = DesignMode.DesignModeEnabled;
#elif XAMARIN
                    _isInDesignMode = false;
#elif WINDOWS_UWP
                    _isInDesignMode = Windows.ApplicationModel.DesignMode.DesignModeEnabled;

#else
                    _isInDesignMode = false;
#endif
                }

                return _isInDesignMode.Value;
            }
        }

#if PORTABLE
        private static bool IsInDesignModePortable()
        {
            // As a portable lib, we need see what framework we're runnign on
            // and use reflection to get the designer value

            var platform = DesignerLibrary.DetectedDesignerLibrary;

            if (platform == DesignerPlatformLibrary.WinRt)
            {
                return IsInDesignModeMetro();
            }

            if (platform == DesignerPlatformLibrary.Silverlight)
            {
                var desMode = IsInDesignModeSilverlight();
                if (!desMode)
                {
                    desMode = IsInDesignModeNet(); // hard to tell these apart in the designer
                }

                return desMode;
            }

            if (platform == DesignerPlatformLibrary.Net)
            {
                return IsInDesignModeNet();
            }

            return false;
        }

        private static bool IsInDesignModeSilverlight()
        {
            try
            {
                var dm = Type.GetType("System.ComponentModel.DesignerProperties, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");

                if (dm is null)
                {
                    return false;
                }

                var dme = dm.GetTypeInfo().GetDeclaredProperty("IsInDesignTool");

                if (dme is null)
                {
                    return false;
                }

                return (bool)dme.GetValue(null, null);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsInDesignModeMetro()
        {
            try
            {
                var dm = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime");

                var dme = dm.GetTypeInfo().GetDeclaredProperty("DesignModeEnabled");
                return (bool)dme.GetValue(null, null);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsInDesignModeNet()
        {
            try
            {
                var dm =
                    Type.GetType(
                        "System.ComponentModel.DesignerProperties, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

                if (dm is null)
                {
                    return false;
                }

                var dmp = dm.GetTypeInfo().GetDeclaredField("IsInDesignModeProperty").GetValue(null);

                var dpd =
                    Type.GetType(
                        "System.ComponentModel.DependencyPropertyDescriptor, WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                var typeFe =
                    Type.GetType("System.Windows.FrameworkElement, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

                if (dpd is null
                    || typeFe is null)
                {
                    return false;
                }

                var fromPropertys = dpd
                    .GetTypeInfo()
                    .GetDeclaredMethods("FromProperty")
                    .ToList();

                if (fromPropertys is null
                    || fromPropertys.Count == 0)
                {
                    return false;
                }

                var fromProperty = fromPropertys
                    .FirstOrDefault(mi => mi.IsPublic && mi.IsStatic && mi.GetParameters().Length == 2);

                if (fromProperty is null)
                {
                    return false;
                }

                var descriptor = fromProperty.Invoke(null, new[] { dmp, typeFe });

                if (descriptor is null)
                {
                    return false;
                }

                var metaProp = dpd.GetTypeInfo().GetDeclaredProperty("Metadata");

                if (metaProp is null)
                {
                    return false;
                }

                var metadata = metaProp.GetValue(descriptor, null);
                var tPropMeta = Type.GetType("System.Windows.PropertyMetadata, WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

                if (metadata is null
                    || tPropMeta is null)
                {
                    return false;
                }

                var dvProp = tPropMeta.GetTypeInfo().GetDeclaredProperty("DefaultValue");

                if (dvProp is null)
                {
                    return false;
                }

                var dv = (bool)dvProp.GetValue(metadata, null);
                return dv;
            }
            catch
            {
                return false;
            }
        }
#endif
    }

    /// <summary>
    /// Enum DesignerPlatformLibrary
    /// </summary>
    internal enum DesignerPlatformLibrary
    {
        /// <summary>
        /// The unknown
        /// </summary>
        Unknown,
        /// <summary>
        /// The net
        /// </summary>
        Net,
        /// <summary>
        /// The win rt
        /// </summary>
        WinRt,
        /// <summary>
        /// The silverlight
        /// </summary>
        Silverlight
    }
}
