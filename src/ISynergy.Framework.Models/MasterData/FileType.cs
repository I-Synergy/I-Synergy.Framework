namespace ISynergy.Framework.Models.MasterData
{
    /// <summary>
    /// Class FileType.
    /// </summary>
    public sealed class FileType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileType"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="description">The description.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="isImage">if set to <c>true</c> [is image].</param>
        /// <param name="contentType">Type of the content.</param>
        public FileType(int id, string description, string extension, bool isImage, string contentType)
        {
            FileTypeId = id;
            Description = description;
            Extension = extension;
            IsImage = isImage;
            ContentType = contentType;
        }

        /// <summary>
        /// Gets or sets the FileTypeId property value.
        /// </summary>
        /// <value>The file type identifier.</value>
        public int FileTypeId { get; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; }

        /// <summary>
        /// Gets or sets the Extension property value.
        /// </summary>
        /// <value>The extension.</value>
        public string Extension { get; }

        /// <summary>
        /// Gets or sets the IsImage property value.
        /// </summary>
        /// <value><c>true</c> if this instance is image; otherwise, <c>false</c>.</value>
        public bool IsImage { get; }

        /// <summary>
        /// Gets or sets the ContentType property value.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType { get; }
    }
}
