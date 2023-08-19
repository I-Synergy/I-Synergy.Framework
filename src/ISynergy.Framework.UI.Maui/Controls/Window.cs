using CommunityToolkit.Maui.Views;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.UI.Controls
{
    public class Window : Popup, IWindow
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(Window), string.Empty);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
                BindingContext = value;
            }
        }

        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(nameof(ViewModel), typeof(IViewModel), typeof(Window), null);

        public IViewModel ViewModel
        {
            get => (IViewModel)GetValue(ViewModelProperty);
            set
            {
                SetValue(ViewModelProperty, value);
                BindingContext = value;
            }
        }

        public Window()
        {
            CanBeDismissedByTappingOutsideOfPopup = false;
            Color = Color.FromArgb("#80000000");
            VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
        }

        #region IDisposable
        // Dispose() calls Dispose(true)
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources, but leave the other methods
        // exactly as they are.
        //~ObservableClass()
        //{
        //    // Finalizer calls Dispose(false)
        //    Dispose(false);
        //}

        // The bulk of the clean-up code is implemented in Dispose(bool)
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                ViewModel?.Dispose();
            }

            // free native resources if there are any.
        }

        public void Close() => throw new NotSupportedException("Close is not supported on the .Net Maui platform.");
        #endregion
    }
}
