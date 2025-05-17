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
    public INotification? Notification { get; protected set; }

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
        if (Notification is not null && Notification.Options is not null && Notification.Options.FreezeOnMouseEnter)
        {
            if (!Notification.Options.UnfreezeOnMouseLeave) // message stay freezed, show close button
            {
                var bord2 = Content as Border;
                if (bord2 is not null)
                {
                    if (Notification.CanClose)
                    {
                        Notification.CanClose = false;
                        var btn = this.FindDescendant<Button>("CloseButton");
                        if (btn is not null)
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
        if (Notification is not null && Notification.Options is not null && Notification.Options.FreezeOnMouseEnter && Notification.Options.UnfreezeOnMouseLeave)
            Notification.CanClose = true;

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
