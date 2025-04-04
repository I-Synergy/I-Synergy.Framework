﻿using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels;

/// <summary>
/// Class ConvertersViewModel.
/// </summary>
public class ConvertersViewModel : ViewModelNavigation<object>
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return BaseCommonServices.LanguageService.GetString("Converters"); } }


    /// <summary>
    /// Gets or sets the File property value.
    /// </summary>
    public byte[] FileBytes
    {
        get => GetValue<byte[]>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the FileDescription property value.
    /// </summary>
    public string Description
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the FileContentType property value.
    /// </summary>
    public string ContentType
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertersViewModel"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger">The logger factory.</param>
    public ConvertersViewModel(
        IContext context,
        IBaseCommonServices commonServices,
        ILogger logger)
        : base(context, commonServices, logger)
    {
        SelectedSoftwareEnvironment = (int)SoftwareEnvironments.Production;
    }

    /// <summary>
    /// Gets or sets the SoftwareEnvironments property value.
    /// </summary>
    /// <value>The software environments.</value>
    public SoftwareEnvironments SoftwareEnvironments
    {
        get { return GetValue<SoftwareEnvironments>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the SelectedSoftwareEnvironment property value.
    /// </summary>
    /// <value>The selected software environment.</value>
    public int SelectedSoftwareEnvironment
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the SoftwareEnvironments property value by enum value.
    /// </summary>
    /// <value>The software environments.</value>
    public SoftwareEnvironments SelectedSoftwareEnvironmentByEnum
    {
        get { return GetValue<SoftwareEnvironments>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the IntegerValue property value.
    /// </summary>
    public int IntegerValue
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the DecimalValue property value.
    /// </summary>
    public decimal DecimalValue
    {
        get => GetValue<decimal>();
        set => SetValue(value);
    }

}
