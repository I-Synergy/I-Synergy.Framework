using ISynergy.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Models.Accounts;

/// <summary>
/// Class Module.
/// Implements the <see cref="ModelBase" />
/// </summary>
/// <seealso cref="ModelBase" />
public class Module : ModelBase
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
    public Guid ModuleId
    {
        get { return GetValue<Guid>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Code property value.
    /// </summary>
    /// <value>The name.</value>
    [Required]
    [StringLength(32)]
    public string Name
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Description property value.
    /// </summary>
    /// <value>The description.</value>
    [Required]
    [StringLength(128)]
    public string Description
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    public override string ToString() => Description;
}
