using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace ISynergy.Framework.UI.Controls
{
    public sealed partial class ImageBrowser : UserControl
    {
        /// <summary>
        /// Gets or sets the Image property value.
        /// </summary>
        /// <value>The file.</value>
        public byte[] FileBytes
        {
            get { return (byte[])GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for File.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileProperty = DependencyProperty.Register(nameof(FileBytes), typeof(byte[]), typeof(ImageBrowser), new PropertyMetadata(Array.Empty<byte>()));

        /// <summary>
        /// Gets or sets the ContentType property value.
        /// </summary>
        /// <value>The content type identifier.</value>
        public string ContentType
        {
            get { return (string)GetValue(ContentTypeProperty); }
            set { SetValue(ContentTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTypeProperty = DependencyProperty.Register(nameof(ContentType), typeof(string), typeof(ImageBrowser), new PropertyMetadata(string.Empty));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(ImageBrowser), new PropertyMetadata(string.Empty));

        /// <summary>
        /// ImageBrowser constructor.
        /// </summary>
        public ImageBrowser()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// browse image as an asynchronous operation.
        /// </summary>
        /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
        private async void Button_Browse_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ServiceLocator.Default.GetInstance<ILanguageService>() is ILanguageService languageService &&
                ServiceLocator.Default.GetInstance<IFileService<FileResult>>() is IFileService<FileResult> fileService)
            {
                var result = await fileService.BrowseFileAsync($"{languageService.GetString("Images")} (Jpeg, Gif, Png)|*.jpg; *.jpeg; *.gif; *.png");

                if (result is not null)
                {
                    FileBytes = result.File;
                    ContentType = result.FilePath.ToContentType();
                    Description = result.FileName;
                }
            };
        }

        /// <summary>
        /// take picture as an asynchronous operation.
        /// </summary>
        /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
        private async void Button_Camera_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ServiceLocator.Default.GetInstance<ICameraService>() is ICameraService cameraService)
            {
                var result = await cameraService.TakePictureAsync();

                if (result is not null)
                {
                    FileBytes = result.File;
                    ContentType = result.FilePath.ToContentType();
                    Description = result.FileName;
                }
            };
        }

        /// <summary>
        /// Clears the image.
        /// </summary>
        private void Button_Clear_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileBytes = null;
            ContentType = string.Empty;
        }

        /// <summary>
        /// paste from clipboard as an asynchronous operation.
        /// </summary>
        /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
        private async void Button_Paste_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ServiceLocator.Default.GetInstance<IClipboardService>() is IClipboardService clipboardService)
            {
                var result = await clipboardService.GetByteArrayFromClipboardImageAsync(Framework.Core.Enumerations.ImageFormats.png);

                if (result is not null)
                {
                    FileBytes = result;
                    ContentType = "image/png";
                    Description = $"FROM_CLIPBOARD_{DateTime.Now}";
                }
            }
        }
    }
}
