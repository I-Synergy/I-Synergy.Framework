using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace ISynergy.Framework.UI.Controls;

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

    public string FileName
    {
        get { return (string)GetValue(FileNameProperty); }
        set { SetValue(FileNameProperty, value); }
    }

    // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName), typeof(string), typeof(ImageBrowser), new PropertyMetadata(string.Empty));

    public DateTimeOffset DateTime
    {
        get { return (DateTimeOffset)GetValue(DateTimeProperty); }
        set { SetValue(DateTimeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DateTimeProperty = DependencyProperty.Register(nameof(DateTime), typeof(DateTimeOffset), typeof(ImageBrowser), new PropertyMetadata(DateTimeOffset.Now));

    /// <summary>
    /// ImageBrowser constructor.
    /// </summary>
    public ImageBrowser()
    {
        InitializeComponent();
    }

    /// <summary>
    /// browse image as an asynchronous operation.
    /// </summary>
    /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
    private async void Button_Browse_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (ServiceLocator.Default.GetInstance<ILanguageService>() is { } languageService &&
            ServiceLocator.Default.GetInstance<IFileService<FileResult>>() is { } fileService)
        {
            var result = await fileService.BrowseFileAsync($"{languageService.GetString("Images")} (Jpeg, Gif, Png, WebP)|*.jpg; *.jpeg; *.gif; *.png; *.webp") ;

            if (result is not null && result.Count > 0)
            {
                FileBytes = result.First().File;
                ContentType = result.First().FilePath.ToContentType();
                FileName = result.First().FileName;
                DateTime = System.DateTime.Now;
            }
        };
    }

    /// <summary>
    /// take picture as an asynchronous operation.
    /// </summary>
    /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
    private async void Button_Camera_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (ServiceLocator.Default.GetInstance<ICameraService>() is { } cameraService)
        {
            var result = await cameraService.TakePictureAsync();

            if (result is not null)
            {
                FileBytes = result.File;
                ContentType = result.FilePath.ToContentType();
                FileName = result.FileName;
                DateTime = System.DateTime.Now;
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
        if (ServiceLocator.Default.GetInstance<IClipboardService>() is { } clipboardService && 
            await clipboardService.GetImageFromClipboardAsync() is { } imageResult)
        {
            FileBytes = imageResult.FileBytes;
            ContentType = imageResult.ContentType;
            FileName = $"FROM_CLIPBOARD_{System.DateTime.Now}";
            DateTime = System.DateTime.Now;
        }
    }
}
