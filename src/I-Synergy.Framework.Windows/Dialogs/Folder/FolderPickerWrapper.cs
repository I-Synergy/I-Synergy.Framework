﻿using ISynergy.Framework.Core.Validation;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace ISynergy.Framework.Windows.Dialogs
{
    /// <summary>
    /// Class wrapping <see cref="FolderPicker" />.
    /// </summary>
    internal sealed class FolderPickerWrapper
    {
        /// <summary>
        /// The picker
        /// </summary>
        private readonly FolderPicker picker;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderPickerWrapper" /> class.
        /// </summary>
        /// <param name="settings">The settings for the folder picker.</param>
        internal FolderPickerWrapper(FolderPickerSettings settings)
        {
            Argument.IsNotNull(nameof(settings), settings);

            picker = new FolderPicker
            {
                CommitButtonText = settings.CommitButtonText,
                SettingsIdentifier = settings.SettingsIdentifier,
                SuggestedStartLocation = settings.SuggestedStartLocation,
                ViewMode = settings.ViewMode
            };

            foreach (var fileTypeFilter in settings.FileTypeFilter)
            {
                picker.FileTypeFilter.Add(fileTypeFilter);
            }
        }

        /// <summary>
        /// Shows the folder picker so that the user can pick a folder.
        /// </summary>
        /// <returns>When the call to this method completes successfully, it returns a
        /// <see cref="StorageFolder" /> object that represents the folder that the user picked.</returns>
        internal IAsyncOperation<StorageFolder> PickSingleFolderAsync() => picker.PickSingleFolderAsync();
    }
}