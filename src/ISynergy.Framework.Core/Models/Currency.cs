using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Currency model which fully supports serialization, property changed notifications,
/// backwards compatibility and error checking.
/// </summary>
public class Currency : ModelBase
{
    /// <summary>
    /// Gets or sets the CurrencyId property value.
    /// </summary>
    /// <value>The currency identifier.</value>
    [Identity]
    [Required]
    public int CurrencyId
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Code property value.
    /// </summary>
    /// <value>The code.</value>
    [Required]
    [StringLength(3)]
    public string Code
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Description property value.
    /// </summary>
    /// <value>The description.</value>
    [Required]
    [StringLength(255)]
    public string Description
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Currency_Symbol property value.
    /// </summary>
    /// <value>The currency symbol.</value>
    [Required]
    [StringLength(3)]
    public string CurrencySymbol
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Rate property value.
    /// </summary>
    /// <value>The rate.</value>
    [Required]
    public decimal Rate
    {
        get { return GetValue<decimal>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Rate_Date property value.
    /// </summary>
    /// <value>The rate date.</value>
    [Required]
    public DateTimeOffset RateDate
    {
        get { return GetValue<DateTimeOffset>(); }
        set { SetValue(value); }
    }
}
