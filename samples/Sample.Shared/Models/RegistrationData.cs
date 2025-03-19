using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Models.Accounts;
using System.ComponentModel.DataAnnotations;

namespace Sample.Models;

/// <summary>
/// Class RegistrationData.
/// Implements the <see cref="BaseModel" />
/// </summary>
/// <seealso cref="BaseModel" />
public class RegistrationData : BaseModel, IRegistrationData
{
    /// <summary>
    /// Gets or sets the ApplicationId property value.
    /// </summary>
    /// <value>The application identifier.</value>
    [Required]
    public int ApplicationId
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the RelationId property value.
    /// </summary>
    /// <value>The relation identifier.</value>
    public Guid RelationId
    {
        get { return GetValue<Guid>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the LicenseName property value.
    /// </summary>
    /// <value>The name of the license.</value>
    [Required]
    public string LicenseName
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Modules property value.
    /// </summary>
    /// <value>The modules.</value>
    [Required]
    public List<Module> Modules
    {
        get { return GetValue<List<Module>>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the UsersAllowed property value.
    /// </summary>
    /// <value>The users allowed.</value>
    [Required]
    public int UsersAllowed
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Email property value.
    /// </summary>
    /// <value>The email.</value>
    [Required]
    public string Email
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Country code (ISO2)
    /// </summary>
    [Required]
    public string CountryCode
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the TimeZoneId property value.
    /// </summary>
    /// <value>The time zone identifier.</value>
    [Required]
    public string TimeZoneId
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Password property value.
    /// </summary>
    /// <value>The password.</value>
    [Required]
    public string Password
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }
}
