using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Base;
using System;

namespace Sample.Models;

/// <summary>
/// Class TestItem.
/// </summary>
public class TestItem : ModelBase
{
    /// <summary>
    /// Gets or sets the Id property value.
    /// </summary>
    /// <value>The identifier.</value>
    public int Id
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Description property value.
    /// </summary>
    /// <value>The description.</value>
    public string Description
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    public TestItem()
    {
        Validator = new Action<IObservableClass>(_ =>
        {
            if (Id < 1)
                AddValidationError(nameof(Id), "Id cannot be zero!");

            if (string.IsNullOrEmpty(Description) || Description.Length < 3)
                AddValidationError(nameof(Description), "Description should be a string with a minimum length of 3 characters!");
        });
    }

    public override string ToString()
    {
        return Description;
    }
}
