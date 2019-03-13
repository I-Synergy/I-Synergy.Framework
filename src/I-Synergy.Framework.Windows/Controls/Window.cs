using GalaSoft.MvvmLight.Messaging;
using ISynergy.Events;
using ISynergy.Mvvm;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ISynergy.Controls
{
    public class Window : ContentDialog, IWindow
    {
        public new IViewModel DataContext
        {
            get { return base.DataContext as IViewModel; }
            set { base.DataContext = value; }
        }

        public Window()
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
                if (VisualTreeHelper.GetChild(parent, i) is FrameworkElement frameworkElement)
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
                    if(DataContext is IViewModelDialog<TEntity> dataContext && !dataContext.IsCancelled)
                    {
                        return true;
                    }

                    return false;
            }
        }
    }
}
