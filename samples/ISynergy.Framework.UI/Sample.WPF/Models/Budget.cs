using ISynergy.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Models;

public class Budget : ModelBase
{
    /// <summary>
    /// Gets or sets the StartingDate property value.
    /// </summary>
    public DateTimeOffset StartingDate
    {
        get => GetValue<DateTimeOffset>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the EndingDate property value.
    /// </summary>
    public DateTimeOffset EndingDate
    {
        get => GetValue<DateTimeOffset>();
        set => SetValue(value);
    }
}
