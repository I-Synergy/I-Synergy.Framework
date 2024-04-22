namespace ISynergy.Framework.Core.Abstractions.Base;

/// <summary>
/// Interface IClassBase
/// </summary>
public interface IBaseClass
{
    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    /// <value>The version.</value>
    int Version { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is deleted.
    /// </summary>
    /// <value><c>true</c> if this instance is deleted; otherwise, <c>false</c>.</value>
    bool IsDeleted { get; set; }
}
