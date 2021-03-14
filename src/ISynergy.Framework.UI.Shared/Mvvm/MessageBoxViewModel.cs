using System;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ISynergy.Framework.UI.ViewModels.Library
{
    /// <summary>
    /// Class MessageBoxViewModel.
    /// Implements the <see cref="ViewModelMessageBox" />
    /// </summary>
    /// <seealso cref="ViewModelMessageBox" />
    public class MessageBoxViewModel : ViewModelMessageBox
    {
        /// <summary>
        /// Gets or sets the MessageImageSource property value.
        /// </summary>
        /// <value>The message image source.</value>
        public ImageSource MessageImageSource
        {
            get { return GetValue<ImageSource>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ContentFlowDirection property value.
        /// </summary>
        /// <value>The content flow direction.</value>
        public FlowDirection ContentFlowDirection
        {
            get { return GetValue<FlowDirection>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the TitleFlowDirection property value.
        /// </summary>
        /// <value>The title flow direction.</value>
        public FlowDirection TitleFlowDirection
        {
            get { return GetValue<FlowDirection>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ContentTextAlignment property value.
        /// </summary>
        /// <value>The content text alignment.</value>
        public TextAlignment ContentTextAlignment
        {
            get { return GetValue<TextAlignment>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="button">The button.</param>
        /// <param name="image">The image.</param>
        /// <param name="result">The result.</param>
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

        /// <summary>
        /// Sets the image source.
        /// </summary>
        /// <param name="image">The image.</param>
        private void SetImageSource(MessageBoxImage image)
        {
            MessageImageSource = (Convert.ToInt32(image)) switch
            {
                16 => new BitmapImage(new Uri("/ISynergy.Framework.UI;component/Assets/Images/Error.png", UriKind.Relative)),
                48 => new BitmapImage(new Uri("/ISynergy.Framework.UI;component/Assets/Images/Warning.png", UriKind.Relative)),
                32 => new BitmapImage(new Uri("/ISynergy.Framework.UI;component/Assets/Images/Question.png", UriKind.Relative)),
                64 => new BitmapImage(new Uri("/ISynergy.Framework.UI;component/Assets/Images/Information.png", UriKind.Relative)),
                _ => null,
            };
        }
    }
}
