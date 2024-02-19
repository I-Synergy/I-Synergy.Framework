using ISynergy.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Models.Accounts;

/// <summary>
/// Class Module.
/// Implements the <see cref="ModelBase" />
/// </summary>
/// <seealso cref="ModelBase" />
public record Module : RecordBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Module"/> class.
    /// </summary>
    public Module()
    {
        ModuleId = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the Module_Id property value.
    /// </summary>
    /// <value>The module identifier.</value>
    [Required]
    public Guid ModuleId { get; set; }

    /// <summary>
    /// Gets or sets the Code property value.
    /// </summary>
    /// <value>The name.</value>
    [Required]
    [StringLength(32)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the Description property value.
    /// </summary>
    /// <value>The description.</value>
    [Required]
    [StringLength(128)]
    public string Description { get; set; }

    public override string ToString() => Description;
}
