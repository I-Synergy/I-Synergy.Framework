using System.Diagnostics;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Windows-specific half of <see cref="DialogService"/>: writes notification fallbacks to the
/// application event log so they show up in Event Viewer and monitoring.
/// </summary>
public partial class DialogService
{
    /// <inheritdoc />
    static partial void WritePlatformEventLog(string message, string title, ref bool handled)
    {
        try
        {
            using var eventLog = new EventLog("Application");
            eventLog.Source = "MAUI DialogService";
            eventLog.WriteEntry($"{title}: {message}", EventLogEntryType.Warning);
            handled = true;
        }
        catch
        {
            handled = false;
        }
    }
}
