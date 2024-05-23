using ISynergy.Framework.Core.Abstractions.Base;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Base;

/// <summary>
/// Base_Class model which fully supports serialization, property changed notifications,
/// backwards compatibility and error checking.
/// </summary>
public abstract class BaseClass : IClass
{
    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    /// <value>The version.</value>
    [Required]
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is deleted.
    /// </summary>
    /// <value><c>true</c> if this instance is deleted; otherwise, <c>false</c>.</value>
    [Required]
    public bool IsDeleted { get; set; }
}
