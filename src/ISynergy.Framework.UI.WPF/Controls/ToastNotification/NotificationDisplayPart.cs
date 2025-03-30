using ISynergy.Framework.UI.Abstractions.Controls.ToastMessages;
using ISynergy.Framework.UI.Controls.ToastNotification.Display;
using ISynergy.Framework.UI.Extensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ISynergy.Framework.UI.Controls.ToastNotification;

public abstract class NotificationDisplayPart : UserControl
{
    protected INotificationAnimator Animator;
    public INotification Notification { get; protected set; }

    protected NotificationDisplayPart()
    {
        Animator = new NotificationAnimator(this, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(300));

        Margin = new Thickness(1);

        Animator.Setup();

        Loaded += OnLoaded;
        MinHeight = 60;
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        var options = Notification.Options;
        if (options != null && options.FreezeOnMouseEnter)
        {
            if (!options.UnfreezeOnMouseLeave) // message stay freezed, show close button
            {
                var bord2 = Content as Border;
                if (bord2 != null)
                {
                    if (Notification.CanClose)
                    {
                        Notification.CanClose = false;
                        var btn = this.FindChild<Button>("CloseButton");
                        if (btn != null)
                        {
                            btn.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            else
            {
                Notification.CanClose = false;
            }
        }
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        var opts = Notification.Options;
        if (opts != null && opts.FreezeOnMouseEnter && opts.UnfreezeOnMouseLeave)
        {
            Notification.CanClose = true;
        }
        base.OnMouseLeave(e);
    }

    public void Bind<TNotification>(TNotification notification) where TNotification : INotification
    {
        Notification = notification;
        DataContext = Notification;
    }

    private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
        Animator.PlayShowAnimation();
    }

    public void OnClose()
    {
        Animator.PlayHideAnimation();
    }
}
