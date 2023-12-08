using ISynergy.Framework.UI.Controls.ToastNotification.Enumerations;

namespace ISynergy.Framework.UI.Options;

public class ToastMessageOptions
{
    public double OffsetX { get; set; } = 25;
    public double OffsetY { get; set; } = 25;
    public Corner Corner { get; set; } = Corner.BottomRight;
    public double Width { get; set; } = 250;
    public bool TopMost { get; set; } = false;
    public int NotificationLifetimeInSeconds { get; set; } = 5;
    public int MaximumNotificationCount { get; set; } = 5;
}
