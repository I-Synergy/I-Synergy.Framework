using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;

namespace Sample.ViewModels
{
    public enum ImagesOrder
    {
        [Description("From left top-down, to right.")]
        TopDownLeftRight,
        [Description("From left to right, top-down")]
        LeftRightTopDown
    }

    public enum ImageFilter
    {
        [Description("*.png")]
        png,
        [Description("*.jpg")]
        jpg,
        [Description("*.jpeg")]
        jpeg,
        [Description("*.gif")]
        gif,
        [Description("*.bmp")]
        bmp
    }

    public class StitchingViewModel : ViewModelNavigation<object>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Stitching"); } }

        /// <summary>
        /// Gets or sets the Columns property value.
        /// </summary>
        public int Columns
        {
            get { return GetValue<int>(); }
            set { SetValue(value, true); }
        }

        /// <summary>
        /// Gets or sets the Rows property value.
        /// </summary>
        public int Rows
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Count property value.
        /// </summary>
        public int Count
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Folder property value.
        /// </summary>
        public string Folder
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Files property value.
        /// </summary>
        public string[] Files
        {
            get { return GetValue<string[]>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Image property value.
        /// </summary>
        public byte[] Image
        {
            get { return GetValue<byte[]>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Order property value.
        /// </summary>
        public ImagesOrder Order
        {
            get { return GetValue<ImagesOrder>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedOrder property value.
        /// </summary>
        public int SelectedOrder
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Filter property value.
        /// </summary>
        public ImageFilter Filter
        {
            get { return GetValue<ImageFilter>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedFilter property value.
        /// </summary>
        public int SelectedFilter
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }


        public RelayCommand BrowseFolder_Command { get; set; }
        public RelayCommand ClearSettings_Command { get; set; }
        public RelayCommand StitchImages_Command { get; set; }

        private readonly ICommonServices _commonServices;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="commonServices"></param>
        /// <param name="logger"></param>
        public StitchingViewModel(
            IContext context,
            ICommonServices commonServices,
            ILogger logger)
            : base(context, commonServices, logger)
        {
            _commonServices = commonServices;

            BrowseFolder_Command = new RelayCommand(async () => await BrowseFolderAsync());
            ClearSettings_Command = new RelayCommand(async () => await ClearSettingsAsync());
            StitchImages_Command = new RelayCommand(async () => await StitchImagesAsync());

            this.Validator = new Action<IObservableClass>(_ =>
            {
                if (string.IsNullOrEmpty(Folder))
                {
                    Properties[nameof(Folder)].Errors.Add($"Value of [{nameof(Folder)}] cannot be null or empty.");
                }

                if (Columns < 1)
                {
                    Properties[nameof(Columns)].Errors.Add($"Value of [{nameof(Columns)}] should be higher than 0.");
                }

                if (Rows < 1)
                {
                    Properties[nameof(Rows)].Errors.Add($"Value of [{nameof(Rows)}] should be higher than 0.");
                }

                if (Count < 1)
                {
                    Properties[nameof(Count)].Errors.Add($"Value of [{nameof(Count)}] should be higher than 0.");
                }
            });

            ClearSettings();
        }

        private Task StitchImagesAsync()
        {
            //try
            //{
            //    BaseCommonServices.BusyService.StartBusy();

            //    if (Validate())
            //    {
            //        get first image to get size
            //        var firstImage = new Bitmap(Files[0]);
            //        var height = firstImage.Height;
            //        var width = firstImage.Width;
            //        firstImage.Dispose();

            //        create canvas from rows and columns
            //                      from
            //        using (var newImage = new Bitmap(width * Columns, height * Rows))
            //        using (var graphics = Graphics.FromImage(newImage))
            //        {
            //            var hdc = graphics.GetHdc();

            //            try
            //            {
            //                switch ((ImagesOrder)SelectedOrder)
            //                {
            //                    case ImagesOrder.TopDownLeftRight:
            //                        for (int i = 0; i < Columns; i++)
            //                        {
            //                            for (int j = 0; j < Rows; j++)
            //                            {
            //                                using (var bitmap = new Bitmap(Files[(i * Rows) + j]))
            //                                {
            //                                    Graphics.FromHdc(hdc).DrawImage(
            //                                        bitmap,
            //                                        width * i,
            //                                        height * j,
            //                                        width,
            //                                        height);
            //                                }
            //                            }
            //                        }
            //                        break;
            //                    case ImagesOrder.LeftRightTopDown:
            //                        for (int i = 0; i < Rows; i++)
            //                        {
            //                            for (int j = 0; j < Columns; j++)
            //                            {
            //                                using (var bitmap = new Bitmap(Files[(Columns * i) + j]))
            //                                {
            //                                    Graphics.FromHdc(hdc).DrawImage(
            //                                        bitmap,
            //                                        width * j,
            //                                        height * i,
            //                                        width,
            //                                        height);
            //                                }
            //                            }
            //                        }
            //                        break;
            //                }
            //            }
            //            finally
            //            {
            //                graphics.ReleaseHdc(hdc);
            //                graphics.Dispose();
            //            }

            //            using (var ms = new MemoryStream())
            //            using (var ws = new WrappingStream(ms))
            //            {
            //                newImage.Save(ms, ImageFormat.Png);
            //                Image = ms.ToArray();
            //                ms.Close();
            //                ms.Dispose();
            //            }

            //            newImage.Dispose();
            //        }
            //    }
            //}
            //finally
            //{
            //    BaseCommonServices.BusyService.EndBusy();
            //}



            return Task.CompletedTask;
        }

        private void ClearSettings()
        {
            SelectedFilter = (int)ImageFilter.png;
            SelectedOrder = (int)ImagesOrder.LeftRightTopDown;
            Folder = string.Empty;
            Files = Array.Empty<string>();
            Count = 0;
            Image = null;
        }

        private Task ClearSettingsAsync()
        {
            ClearSettings();

            return Task.CompletedTask;
        }

        private Task BrowseFolderAsync()
        {
            //var folder = await _commonServices.FileService.OpenFolderDialogAsync();

            //if (!string.IsNullOrEmpty(folder))
            //{
            //    Folder = folder;

            //    var filter = string.Empty;

            //    switch ((ImageFilter)SelectedFilter)
            //    {
            //        case ImageFilter.jpg:
            //            filter = "*.jpg";
            //            break;
            //        case ImageFilter.jpeg:
            //            filter = "*.jpeg";
            //            break;
            //        case ImageFilter.gif:
            //            filter = "*.gif";
            //            break;
            //        case ImageFilter.bmp:
            //            filter = "*.bmp";
            //            break;
            //        default:
            //            filter = "*.png";
            //            break;
            //    }

            //    var files = await _commonServices.FileService.GetFilesFromFolderAsync(folder, filter, SearchOption.TopDirectoryOnly);
            //    Array.Sort(files);
            //    Files = files;
            //    Count = Files.Length;
            //}

            return Task.CompletedTask;
        }
    }
}
