﻿using ISynergy.Framework.Mvvm.Abstractions.Services;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.ViewModels;

/// <summary>
/// Class MapsViewModel.
/// Implements the <see cref="ViewModelDialog{Object}" />
/// </summary>
/// <seealso cref="ViewModelDialog{Object}" />
public class MapsViewModel : ViewModelDialog<object>
{
    /// <summary>
    /// Gets or sets the Title property value.
    /// </summary>
    /// <value>The title.</value>
    public override string Title
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Locations property value.
    /// </summary>
    /// <value>The locations.</value>
    public ObservableCollection<object> Locations
    {
        get { return GetValue<ObservableCollection<object>>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the TimeToArrival property value.
    /// </summary>
    /// <value>The time to arrival.</value>
    public double TimeToArrival
    {
        get { return GetValue<double>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the DistcanceToArrival property value.
    /// </summary>
    /// <value>The distcance to arrival.</value>
    public double DistcanceToArrival
    {
        get { return GetValue<double>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Content property value.
    /// </summary>
    /// <value>The content.</value>
    public string Content
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Name property value.
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Address property value.
    /// </summary>
    /// <value>The address.</value>
    public string Address
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MapsViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="name">The name.</param>
    /// <param name="address">The address.</param>
    public MapsViewModel(
        ICommonServices commonServices,
        string name,
        string address)
        : base(commonServices)
    {
        Locations = new ObservableCollection<object>();
        Title = address;
        Address = address;
        Name = name;
    }
}
