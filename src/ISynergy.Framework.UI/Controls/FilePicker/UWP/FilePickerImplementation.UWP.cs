#if NETFX_CORE
using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.UI.Controls.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.System;

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Implementation for file picking on UWP
    /// </summary>
    public class FilePickerImplementation : IFilePicker
    {
        /// <summary>
        /// Implementation for picking a file on UWP platform.
        /// </summary>
        /// <param name="allowedTypes">
        /// Specifies one or multiple allowed types. When null, all file types
        /// can be selected while picking.
        /// On UWP, specify a list of extensions, like this: ".jpg", ".png".
        /// </param>
        /// <returns>
        /// File data object, or null when user cancelled picking file
        /// </returns>
        public async Task<FileResult> PickFileAsync(string[] allowedTypes = null)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.List,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };

            if (allowedTypes != null)
            {
                var hasAtleastOneType = false;

                foreach (var type in allowedTypes)
                {
                    if (type.StartsWith("."))
                    {
                        picker.FileTypeFilter.Add(type);
                        hasAtleastOneType = true;
                    }
                }

                if (!hasAtleastOneType)
                {
                    picker.FileTypeFilter.Add("*");
                }
            }
            else
            {
                picker.FileTypeFilter.Add("*");
            }

            if(await picker.PickSingleFileAsync() is StorageFile file)
            {
                StorageApplicationPermissions.FutureAccessList.Add(file);

                return new FileResult(
                    file.Path, 
                    file.Name,
                    () => file.OpenStreamForReadAsync().GetAwaiter().GetResult());
            }

            return null;
        }

        /// <summary>
        /// UWP implementation of saving a picked file to the app's local folder directory.
        /// </summary>
        /// <param name="fileToSave">filename for file to save</param>
        /// <param name="file">file data for file to save</param>
        /// <returns>true when file was saved successfully, false when not</returns>
        public async Task<FileResult> SaveFileAsync(string fileToSave, byte[] file)
        {
            var createdFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    fileToSave,
                    CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteBytesAsync(createdFile, file);

            return new FileResult(
                    createdFile.Path,
                    createdFile.Name,
                    () => createdFile.OpenStreamForReadAsync().GetAwaiter().GetResult());
        }

        /// <summary>
        /// UWP implementation of OpenFile(), opening a file already stored in the app's local
        /// folder directory.
        /// storage.
        /// </summary>
        /// <param name="fileToOpen">relative filename of file to open</param>
        public async void OpenFile(string fileToOpen)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileToOpen);

                if (file != null)
                {
                    await Launcher.LaunchFileAsync(file);
                }
            }
            catch (FileNotFoundException)
            {
                // ignore exceptions
            }
            catch (Exception)
            {
                // ignore exceptions
            }
        }
    }
}
#endif
