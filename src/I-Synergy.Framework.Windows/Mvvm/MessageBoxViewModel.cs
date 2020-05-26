using System;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ISynergy.Framework.Windows.ViewModels.Library
{
    public class MessageBoxViewModel : ViewModelMessageBox
    {
        /// <summary>
        /// Gets or sets the MessageImageSource property value.
        /// </summary>
        public ImageSource MessageImageSource
        {
            get { return GetValue<ImageSource>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ContentFlowDirection property value.
        /// </summary>
        public FlowDirection ContentFlowDirection
        {
            get { return GetValue<FlowDirection>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the TitleFlowDirection property value.
        /// </summary>
        public FlowDirection TitleFlowDirection
        {
            get { return GetValue<FlowDirection>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ContentTextAlignment property value.
        /// </summary>
        public TextAlignment ContentTextAlignment
        {
            get { return GetValue<TextAlignment>(); }
            set { SetValue(value); }
        }

        public MessageBoxViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            string message,
            string title,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information,
            MessageBoxResult result = MessageBoxResult.OK)
        : base(context, commonServices, loggerFactory, message, title, button, image)
        {
            SetButtonVisibility(button);
            SetImageSource(image);
            SetButtonDefault(result);
        }

        private void SetImageSource(MessageBoxImage image)
        {
            MessageImageSource = (Convert.ToInt32(image)) switch
            {
                16 => new BitmapImage(new Uri("/I-Synergy.Framework.Windows;component/Assets/Images/Error.png", UriKind.Relative)),
                48 => new BitmapImage(new Uri("/I-Synergy.Framework.Windows;component/Assets/Images/Warning.png", UriKind.Relative)),
                32 => new BitmapImage(new Uri("/I-Synergy.Framework.Windows;component/Assets/Images/Question.png", UriKind.Relative)),
                64 => new BitmapImage(new Uri("/I-Synergy.Framework.Windows;component/Assets/Images/Information.png", UriKind.Relative)),
                _ => null,
            };
        }
    }
}
