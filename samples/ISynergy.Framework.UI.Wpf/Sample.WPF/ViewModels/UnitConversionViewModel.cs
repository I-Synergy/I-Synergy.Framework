using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Enumerations;
using ISynergy.Framework.Physics.Extensions;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using Sample.Abstractions.Services;
using System.ComponentModel;

namespace Sample.ViewModels;

public class UnitConversionViewModel : ViewModelNavigation<object>
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return "Unit Conversion"; } }

    private readonly IUnitConversionService _unitConversionService;

    /// <summary>
    /// Gets or sets the Input property value.
    /// </summary>
    public double Input
    {
        get { return GetValue<double>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the input Unit property value.
    /// </summary>
    public string InputUnit
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Output property value.
    /// </summary>
    public double Output
    {
        get { return GetValue<double>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the output Unit property value.
    /// </summary>
    public string OutputUnit
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }


    /// <summary>
    /// Gets or sets the Units property value.
    /// </summary>
    public Units Items
    {
        get { return GetValue<Units>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Selected input Unit property value.
    /// </summary>
    public int SelectedInputUnit
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Selected output Unit property value.
    /// </summary>
    public int SelectedOutputUnit
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    public UnitConversionViewModel(
        IContext context,
        ICommonServices commonServices,
        ILogger logger,
        IUnitConversionService unitConversionService)
        : base(context, commonServices, logger)
    {
        _unitConversionService = unitConversionService;

        SelectedInputUnit = (int)Units.centimetre;
        SelectedOutputUnit = (int)Units.metre;
    }

    public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName.Equals(nameof(SelectedInputUnit)))
        {
            InputUnit = ((Units)SelectedInputUnit).GetSymbol();
        }

        if (e.PropertyName.Equals(nameof(SelectedOutputUnit)))
        {
            OutputUnit = ((Units)SelectedOutputUnit).GetSymbol();
        }
    }

    public override Task SubmitAsync(object e, bool validateUnderlayingProperties = true)
    {
        Output = _unitConversionService.Convert((Units)SelectedInputUnit, Input, (Units)SelectedOutputUnit);
        return Task.CompletedTask;
    }
}
