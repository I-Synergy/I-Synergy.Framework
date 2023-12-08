using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;
using ISynergy.Framework.UI.Controls.ToastNotification.Options;
using System.Windows.Threading;

namespace ISynergy.Framework.UI.Controls.ToastNotification;

public class NotifierConfiguration
{
    public IPositionProvider PositionProvider { get; set; }
    public INotificationsLifetimeSupervisor LifetimeSupervisor { get; set; }
    public Dispatcher Dispatcher { get; set; }
    public DisplayOptions DisplayOptions { get; }
    public IKeyboardEventHandler KeyboardEventHandler { get; set; }

    public NotifierConfiguration()
    {
        DisplayOptions = new DisplayOptions
        {
            Width = 250,
            TopMost = true
        };
    }
}
