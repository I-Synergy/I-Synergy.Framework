namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface IFileSupport
    /// </summary>
    public interface IFileSupport
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        string FileName { get; set; }
        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        string Filter { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [add extension].
        /// </summary>
        /// <value><c>true</c> if [add extension]; otherwise, <c>false</c>.</value>
        bool AddExtension { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [check file exists].
        /// </summary>
        /// <value><c>true</c> if [check file exists]; otherwise, <c>false</c>.</value>
        bool CheckFileExists { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [check path exists].
        /// </summary>
        /// <value><c>true</c> if [check path exists]; otherwise, <c>false</c>.</value>
        bool CheckPathExists { get; set; }
        /// <summary>
        /// Gets or sets the index of the filter.
        /// </summary>
        /// <value>The index of the filter.</value>
        int FilterIndex { get; set; }
        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        /// <value>The initial directory.</value>
        string InitialDirectory { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [validate names].
        /// </summary>
        /// <value><c>true</c> if [validate names]; otherwise, <c>false</c>.</value>
        bool ValidateNames { get; set; }
    }
}
