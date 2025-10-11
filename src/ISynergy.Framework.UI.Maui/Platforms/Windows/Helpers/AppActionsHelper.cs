using System.Text;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI.Helpers;

public static class AppActionsHelper
{
    private const string AppActionPrefix = "XE_APP_ACTIONS-";

    public static string GetAppActionId(string arguments)
    {
        if (arguments?.StartsWith(AppActionPrefix) ?? false)
            return Encoding.Default.GetString(Convert.FromBase64String(arguments.Substring(AppActionPrefix.Length)));

        return default;
    }
}

