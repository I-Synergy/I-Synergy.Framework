namespace ISynergy.Framework.Core.Abstractions.Base;

public interface IRecord
{
    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    /// <value>The version.</value>
    int Version { get; set; }
    /// <summary>
    /// Gets or sets the memo.
    /// </summary>
    /// <value>The memo.</value>
    string Memo { get; set; }
    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    /// <value>The created date.</value>
    DateTimeOffset CreatedDate { get; set; }
    /// <summary>
    /// Gets or sets the changed date.
    /// </summary>
    /// <value>The changed date.</value>
    DateTimeOffset? ChangedDate { get; set; }
    /// <summary>
    /// Gets or sets the created by.
    /// </summary>
    /// <value>The created by.</value>
    string CreatedBy { get; set; }
    /// <summary>
    /// Gets or sets the changed by.
    /// </summary>
    /// <value>The changed by.</value>
    string ChangedBy { get; set; }
}
