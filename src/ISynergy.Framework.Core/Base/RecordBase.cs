using ISynergy.Framework.Core.Abstractions.Base;

namespace ISynergy.Framework.Core.Base;

public abstract record RecordBase : IRecordBase
{
    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    /// <value>The version.</value>
    public int Version { get; set; }
    /// <summary>
    /// Gets or sets the memo.
    /// </summary>
    /// <value>The memo.</value>
    public string Memo { get; set; }
    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    /// <value>The created date.</value>
    public DateTimeOffset CreatedDate { get; set; }
    /// <summary>
    /// Gets or sets the changed date.
    /// </summary>
    /// <value>The changed date.</value>
    public DateTimeOffset? ChangedDate { get; set; }
    /// <summary>
    /// Gets or sets the created by.
    /// </summary>
    /// <value>The created by.</value>
    public string CreatedBy { get; set; }
    /// <summary>
    /// Gets or sets the changed by.
    /// </summary>
    /// <value>The changed by.</value>
    public string ChangedBy { get; set; }
}
