using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.EntityFramework.Base;

/// <summary>
/// BaseEntity model which fully supports serialization, property changed notifications,
/// backwards compatibility and error checking.
/// </summary>
public abstract class BaseEntity : BaseClass, IEntity
{
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

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class.
    /// </summary>
    protected BaseEntity()
    {
        CreatedBy = string.Empty;
    }
}
