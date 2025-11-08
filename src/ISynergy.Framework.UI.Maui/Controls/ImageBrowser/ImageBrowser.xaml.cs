using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Constants;
using FileResult = ISynergy.Framework.Core.Models.Results.FileResult;

namespace ISynergy.Framework.UI.Controls;

public partial class ImageBrowser : ContentView
{
    /// <summary>
    /// Gets or sets the Image property value.
    /// </summary>
    /// <value>The file.</value>
    public byte[] FileBytes
    {
        get { return (byte[])GetValue(FileBytesProperty); }
        set { SetValue(FileBytesProperty, value); }
    }

    // Using a BindableProperty as the backing store for File. This enables animation, styling, binding, etc...
    public static readonly BindableProperty FileBytesProperty = BindableProperty.Create(nameof(FileBytes), typeof(byte[]), typeof(ImageBrowser), Array.Empty<byte>());

    /// <summary>
    /// Gets or sets the ContentType property value.
    /// </summary>
    /// <value>The content type identifier.</value>
    public string ContentType
    {
        get { return (string)GetValue(ContentTypeProperty); }
        set { SetValue(ContentTypeProperty, value); }
    }

    // Using a BindableProperty as the backing store for ContentType. This enables animation, styling, binding, etc...
    public static readonly BindableProperty ContentTypeProperty = BindableProperty.Create(nameof(ContentType), typeof(string), typeof(ImageBrowser), string.Empty);

    public string FileName
    {
        get { return (string)GetValue(FileNameProperty); }
        set { SetValue(FileNameProperty, value); }
    }

    // Using a BindableProperty as the backing store for FileName. This enables animation, styling, binding, etc...
    public static readonly BindableProperty FileNameProperty = BindableProperty.Create(nameof(FileName), typeof(string), typeof(ImageBrowser), string.Empty);

    public DateTimeOffset DateTime
    {
        get { return (DateTimeOffset)GetValue(DateTimeProperty); }
        set { SetValue(DateTimeProperty, value); }
    }

    // Using a BindableProperty as the backing store for FileName. This enables animation, styling, binding, etc...
    public static readonly BindableProperty DateTimeProperty = BindableProperty.Create(nameof(DateTime), typeof(DateTimeOffset), typeof(ImageBrowser), DateTimeOffset.Now);

    public ImageBrowser()
    {
        InitializeComponent();
    }

    private async void PasteButton_Clicked(object sender, EventArgs e)
    {
        if (ServiceLocator.Default.GetRequiredService<IClipboardService>() is { } clipboardService &&
            await clipboardService.GetImageFromClipboardAsync() is { } imageResult &&
            imageResult is not null)
        {
            FileBytes = imageResult.FileBytes ?? Array.Empty<byte>();
            ContentType = imageResult.ContentType ?? string.Empty;
            FileName = $"FROM_CLIPBOARD_{System.DateTime.Now}";
            DateTime = System.DateTime.Now;
        }
    }

    private void ClearButton_Clicked(object sender, EventArgs e)
    {
        FileBytes = Array.Empty<byte>();
        ContentType = string.Empty;
    }

    private async void CameraButton_Clicked(object sender, EventArgs e)
    {
        if (ServiceLocator.Default.GetRequiredService<ICameraService>() is { } cameraService)
        {
            var result = await cameraService.TakePictureAsync();

            if (result is not null)
            {
                FileBytes = result.File;
                ContentType = result.FilePath.ToContentType();
                FileName = result.FileName;
                DateTime = System.DateTime.Now;
            }
        }
        ;
    }

    private async void BrowseButton_Clicked(object sender, EventArgs e)
    {
        if (ServiceLocator.Default.GetRequiredService<ILanguageService>() is { } languageService &&
            ServiceLocator.Default.GetRequiredService<IFileService<FileResult>>() is { } fileService)
        {
            var result = await fileService.BrowseFileAsync(FileTypeConstants.FileTypesImages);

            if (result is not null && result.Count > 0 && result[0] is not null)
            {
                FileBytes = result[0].File;
                ContentType = result[0].FilePath.ToContentType();
                FileName = result[0].FileName;
                DateTime = System.DateTime.Now;
            }
        }
        ;
    }
}