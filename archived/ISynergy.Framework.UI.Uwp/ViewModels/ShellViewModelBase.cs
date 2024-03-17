using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ISynergy.Framework.UI.ViewModels
{
    /// <summary>
    /// Class ShellViewModelBase.
    /// Implements the <see cref="ViewModel" />
    /// Implements the <see cref="IShellViewModel" />
    /// </summary>
    /// <seealso cref="ViewModel" />
    /// <seealso cref="IShellViewModel" />
    public abstract partial class ShellViewModelBase
    {
        /// <summary>
        /// Gets or sets the DisplayMode property value.
        /// </summary>
        /// <value>The display mode.</value>
        public SplitViewDisplayMode DisplayMode
        {
            get => GetValue<SplitViewDisplayMode>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the SelectedForeground property value.
        /// </summary>
        /// <value>The color of the foreground.</value>
        public SolidColorBrush ForegroundColor
        {
            get => GetValue<SolidColorBrush>() ?? GetStandardTextColorBrush();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the state changed command.
        /// </summary>
        /// <value>The state changed command.</value>
        public Command<VisualStateChangedEventArgs> StateChanged_Command { get; set; }

        private void InitializeUI()
        {
            ForegroundColor = new SolidColorBrush(ColorHelper.HexStringToColor(_themeService.Style.Color));
            StateChanged_Command = new Command<VisualStateChangedEventArgs>(args => GoToState(args.NewState.Name));
        }

        /// <summary>
        /// Gets the standard text color brush.
        /// </summary>
        /// <returns>SolidColorBrush.</returns>
        private SolidColorBrush GetStandardTextColorBrush()
        {
            var brush = Application.Current.Resources["SystemControlForegroundBaseHighBrush"] as SolidColorBrush;

            if (!_themeService.IsLightThemeEnabled)
            {
                brush = Application.Current.Resources["SystemControlForegroundAltHighBrush"] as SolidColorBrush;
            }

            return brush;
        }

        /// <summary>
        /// Goes to state.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        protected void GoToState(string stateName)
        {
            switch (stateName)
            {
                case PanoramicStateName:
                    DisplayMode = SplitViewDisplayMode.CompactInline;
                    break;
                case WideStateName:
                    DisplayMode = SplitViewDisplayMode.CompactInline;
                    break;
                case NarrowStateName:
                    DisplayMode = SplitViewDisplayMode.Overlay;
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the Wallpaper property value.
        /// </summary>
        /// <value>The wallpaper.</value>
        public ImageSource Wallpaper
        {
            get => GetValue<ImageSource>();
            set => SetValue(value);
        }
    }
}
