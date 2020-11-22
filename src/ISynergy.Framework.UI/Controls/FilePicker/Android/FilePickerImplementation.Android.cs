#if __ANDROID__
using Android.App;
using Android.Content;
using Android.Runtime;
using Java.IO;
using ISynergy.Framework.UI.Controls.Abstractions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.UI.Controls.Events;
using ISynergy.Framework.Mvvm.Models;
using System.Linq;

// Adds permission for READ_EXTERNAL_STORAGE to the AndroidManifest.xml of the app project without
// the user of the plugin having to add it by himself/herself.
[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Implementation for file picking on Android
    /// </summary>
    [Preserve(AllMembers = true)]
    public class FilePickerImplementation : IFilePicker
    {
        /// <summary>
        /// Android context to use for picking
        /// </summary>
        private readonly Context _context;

        /// <summary>
        /// Request ID for current picking call
        /// </summary>
        private int _requestId;

        /// <summary>
        /// Task completion source for task when finished picking
        /// </summary>
        private TaskCompletionSource<FileResult> _completionSource;

        /// <summary>
        /// Creates a new file picker implementation
        /// </summary>
        public FilePickerImplementation()
        {
            _context = Application.Context;
        }

        /// <summary>
        /// Implementation for picking a file on Android.
        /// </summary>
        /// <param name="allowedTypes">
        /// Specifies one or multiple allowed types. When null, all file types
        /// can be selected while picking.
        /// On Android you can specify one or more MIME types, e.g.
        /// "image/png"; also wild card characters can be used, e.g. "image/*".
        /// </param>
        /// <returns>
        /// File data object, or null when user cancelled picking file
        /// </returns>
        public Task<FileResult> PickFileAsync(string[] allowedTypes) =>
            PickFileAsync(allowedTypes, Intent.ActionGetContent);

        /// <summary>
        /// File picking implementation
        /// </summary>
        /// <param name="allowedTypes">list of allowed types; may be null</param>
        /// <param name="action">Android intent action to use; unused</param>
        /// <returns>picked file data, or null when picking was cancelled</returns>
        private Task<FileResult> PickFileAsync(string[] allowedTypes, string action)
        {
            var id = GetRequestId();

            var ntcs = new TaskCompletionSource<FileResult>(id);

            var previousTcs = Interlocked.Exchange(ref _completionSource, ntcs);
            if (previousTcs != null)
            {
                previousTcs.TrySetResult(null);
            }

            try
            {
                var pickerIntent = new Intent(_context, typeof(FilePickerActivity));
                pickerIntent.SetFlags(ActivityFlags.NewTask);

                pickerIntent.PutExtra(FilePickerActivity.ExtraAllowedTypes, allowedTypes);

                _context.StartActivity(pickerIntent);

                EventHandler<FilePickerEventArgs> handler = null;
                EventHandler<FilePickerCancelledEventArgs> cancelledHandler = null;

                handler = (s, e) =>
                {
                    var tcs = Interlocked.Exchange(ref _completionSource, null);

                    FilePickerActivity.FilePickCancelled -= cancelledHandler;
                    FilePickerActivity.FilePicked -= handler;

                    tcs?.SetResult(new FileResult(
                        e.FilePath,
                        e.FileName,
                        () =>
                        {
                            if (IOUtil.IsMediaStore(e.FilePath))
                            {
                                var contentUri = Android.Net.Uri.Parse(e.FilePath);
                                return Application.Context.ContentResolver.OpenInputStream(contentUri);
                            }
                            else
                            {
                                return System.IO.File.OpenRead(e.FilePath);
                            }
                        }));
                };

                cancelledHandler = (s, e) =>
                {
                    var tcs = Interlocked.Exchange(ref _completionSource, null);

                    FilePickerActivity.FilePickCancelled -= cancelledHandler;
                    FilePickerActivity.FilePicked -= handler;

                    if (e?.Exception != null)
                    {
                        tcs?.SetException(e.Exception);
                    }
                    else
                    {
                        tcs?.SetResult(null);
                    }
                };

                FilePickerActivity.FilePickCancelled += cancelledHandler;
                FilePickerActivity.FilePicked += handler;
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                _completionSource.SetException(ex);
            }

            return _completionSource.Task;
        }

        /// <summary>
        /// Returns a new request ID for a new call to PickFile()
        /// </summary>
        /// <returns>new request ID</returns>
        private int GetRequestId()
        {
            int id = _requestId;

            if (_requestId == int.MaxValue)
            {
                _requestId = 0;
            }
            else
            {
                _requestId++;
            }

            return id;
        }

        /// <summary>
        /// Android implementation of saving a picked file to the external storage directory.
        /// </summary>
        /// <param name="fileToSave">filename for file to save</param>
        /// <param name="file">file data for file to save</param>
        /// <returns>true when file was saved successfully, false when not</returns>
        public Task<FileResult> SaveFileAsync(string fileToSave, byte[] file)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var myFile = new File(Android.OS.Environment.ExternalStorageDirectory, fileToSave);
#pragma warning restore CS0618 // Type or member is obsolete

            if (!myFile.Exists())
            {
                using var fos = new FileOutputStream(myFile.Path);
                fos.Write(file);
                fos.Close();
            }

            return Task.FromResult(new FileResult(
                myFile.Path,
                myFile.Name,
                () =>
                {
                    if (IOUtil.IsMediaStore(myFile.Path))
                    {
                        var contentUri = Android.Net.Uri.Parse(myFile.Path);
                        return Application.Context.ContentResolver.OpenInputStream(contentUri);
                    }
                    else
                    {
                        return System.IO.File.OpenRead(myFile.Path);
                    }
                }));
        }

        /// <summary>
        /// Android implementation of opening a file by using ActionView intent.
        /// </summary>
        /// <param name="fileToOpen">file to open in viewer</param>
        public void OpenFile(File fileToOpen)
        {
            var uri = Android.Net.Uri.FromFile(fileToOpen);
            var intent = new Intent();
            var mime = IOUtil.GetMimeType(uri.ToString());

            intent.SetAction(Intent.ActionView);
            intent.SetDataAndType(uri, mime);
            intent.SetFlags(ActivityFlags.NewTask);

            _context.StartActivity(intent);
        }

        /// <summary>
        /// Android implementation of OpenFile(), opening a file already stored on external
        /// storage.
        /// </summary>
        /// <param name="fileToOpen">relative filename of file to open</param>
        public void OpenFile(string fileToOpen)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var myFile = new File(Android.OS.Environment.ExternalStorageDirectory, fileToOpen);
#pragma warning restore CS0618 // Type or member is obsolete
            OpenFile(myFile);
        }
    }
}
#endif
