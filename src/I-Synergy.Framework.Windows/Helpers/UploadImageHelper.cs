﻿using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace ISynergy.Helpers
{
    public static class UploadImageHelper
    {
        public static async Task<string> UploadImageAsync()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            return await SaveImageAsync(file);
        }

        private static async Task<string> SaveImageAsync(StorageFile file)
        {
            if (file == null)
            {
                return string.Empty;
            }

            var appInstalledFolder = Package.Current.InstalledLocation;
            var assets = await appInstalledFolder.GetFolderAsync("Assets");

            var targetFile = await assets.CreateFileAsync(file.Name, CreationCollisionOption.GenerateUniqueName);
            await file.CopyAndReplaceAsync(targetFile);
            return targetFile.Path;
        }
    }
}
