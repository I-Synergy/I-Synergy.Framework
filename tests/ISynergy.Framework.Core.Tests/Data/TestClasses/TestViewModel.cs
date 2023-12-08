using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Base;
using System;

namespace ISynergy.Framework.Core.Data.Tests.TestClasses;

public class TestViewModel : ObservableClass
{

    /// <summary>
    /// Gets or sets the SelectedItem property value.
    /// </summary>
    public Product SelectedItem
    {
        get => GetValue<Product>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the ExtraItem property value.
    /// </summary>
    public Product ExtraItem
    {
        get => GetValue<Product>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the IsTest property value.
    /// </summary>
    public bool IsTest
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public TestViewModel()
    {
        Validator = new Action<IObservableClass>(arg =>
        {
            if (arg is TestViewModel viewModel)
            {
                if (viewModel.SelectedItem is null)
                {
                    AddValidationError(nameof(SelectedItem), "SelectedItem cannot be null.");
                }
            }
        });
    }

    public void Submit()
    {
        if (Validate())
        {
        }
    }
}
