using ISynergy.Framework.UI.Theme;

namespace ISynergy.Framework.UI.Extensions
{
    public static class ThemeExtensions
    {
        public static void ApplyDarkTheme(this ResourceDictionary resources)
        {
            if (resources != null)
            {
                var mergedDictionaries = resources.MergedDictionaries;
                var lightTheme = mergedDictionaries.OfType<LightTheme>().FirstOrDefault();
                
                if (lightTheme != null)
                    mergedDictionaries.Remove(lightTheme);
                
                mergedDictionaries.Add(new DarkTheme()
                {
                    ["Primary"] = Application.AccentColor,
                    ["colorPrimary"] = Application.AccentColor,
                    ["colorAccent"] = Application.AccentColor,
                    ["Secondary"] = Application.AccentColor.AddLuminosity(0.25f),
                    ["Tertiary"] = Application.AccentColor.AddLuminosity(-0.25f),
                    ["colorPrimaryDark"] = Application.AccentColor.AddLuminosity(-0.25f)
                });

                Application.Current.UserAppTheme = AppTheme.Dark;
            }
        }

        public static void ApplyLightTheme(this ResourceDictionary resources)
        {
            if (resources != null)
            {
                var mergedDictionaries = resources.MergedDictionaries;

                var darkTheme = mergedDictionaries.OfType<DarkTheme>().FirstOrDefault();
                
                if (darkTheme != null)
                    mergedDictionaries.Remove(darkTheme);
                
                mergedDictionaries.Add(new LightTheme()
                {
                    ["Primary"] = Application.AccentColor,
                    ["colorPrimary"] = Application.AccentColor,
                    ["colorAccent"] = Application.AccentColor,
                    ["Secondary"] = Application.AccentColor.AddLuminosity(0.25f),
                    ["Tertiary"] = Application.AccentColor.AddLuminosity(-0.25f),
                    ["colorPrimaryDark"] = Application.AccentColor.AddLuminosity(-0.25f)
                });

                Application.Current.UserAppTheme = AppTheme.Light;
            }
        }
    }
}
