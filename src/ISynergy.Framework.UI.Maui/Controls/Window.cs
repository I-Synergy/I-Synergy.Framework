using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Mopups.Animations;
using Mopups.Enums;
using Mopups.Pages;

namespace ISynergy.Framework.UI.Controls
{
    public class Window : PopupPage, IWindow
    {
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
            CloseWhenBackgroundIsClicked = false;

            BackgroundColor = Color.FromArgb("#80000000");

            Animation = new ScaleAnimation
            {
                DurationIn = 700,
                EasingIn = Easing.BounceOut,
                PositionIn = MoveAnimationOptions.Bottom,
                PositionOut = MoveAnimationOptions.Center,
                ScaleIn = 1,
                ScaleOut = 0.7
            };

            Loaded += Window_Loaded;
        }

        private async void Window_Loaded(object sender, EventArgs e)
        {
            Loaded -= Window_Loaded;

            if (ViewModel is not null)
                await ViewModel.InitializeAsync();
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
