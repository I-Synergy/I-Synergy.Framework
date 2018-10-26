using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.ViewModels.Base;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ISynergy.Controls
{
    public class Window : ContentDialog, IWindow
    {
        public object Owner { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Window() : base()
        {
            Messenger.Default.Register<CloseWindowsMessage>(this, (e) =>
            {
                if (DataContext is null || DataContext.Equals(e.Sender))
                {
                    Close();
                }
            });
        }

        public void Close()
        {
            base.Hide();
        }

        private static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            if (count < 1)
            {
                return null;
            }

            for (var i = 0; i < count; i++)
            {
                var frameworkElement = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;

                if (frameworkElement != null)
                {
                    if (frameworkElement.Name == name)
                    {
                        return frameworkElement;
                    }

                    frameworkElement = GetDescendantFromName(frameworkElement, name);

                    if (frameworkElement != null)
                    {
                        return frameworkElement;
                    }
                }
            }

            return null;
        }

        public async Task<bool?> ShowAsync<TEntity>()
            where TEntity : class, new()
        {
            switch (await base.ShowAsync())
            {
                case ContentDialogResult.Primary:
                    return true;
                case ContentDialogResult.Secondary:
                    return false;
                default:
                    if(this.DataContext != null && this.DataContext is IViewModelDialog<TEntity>)
                    {
                        if (!(this.DataContext as IViewModelDialog<TEntity>).IsCancelled)
                        {
                            return true;
                        }
                    }

                    return false;
            }
        }
    }
}
