using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// TileItem model which fully supports serialization, property changed notifications,
/// backwards compatibility and error checking.
/// </summary>
public class TileItem : BaseModel
{
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
    /// Gets or sets the CommodityGroup property value.
    /// </summary>
    /// <value>The group.</value>
    public string Group
    {
        get { return GetValue<string>(); }
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

    /// <summary>
    /// Gets or sets the Details property value.
    /// </summary>
    /// <value>The details.</value>
    public string Details
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Header property value.
    /// </summary>
    /// <value>The header.</value>
    public string Header
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Quantity property value.
    /// </summary>
    /// <value>The quantity.</value>
    public string Quantity
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Source property value.
    /// </summary>
    /// <value>The source.</value>
    public string Source
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the IsEnabled property value.
    /// </summary>
    /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
    public bool IsEnabled
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the IsTransparent property value.
    /// </summary>
    /// <value><c>true</c> if this instance is transparent; otherwise, <c>false</c>.</value>
    public bool IsTransparent
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the IsTimer property value.
    /// </summary>
    /// <value><c>true</c> if this instance is timer; otherwise, <c>false</c>.</value>
    public bool IsTimer
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    // Properties should not return arrays
    /// <summary>
    /// Gets or sets the Wallpaper property value.
    /// </summary>
    /// <value>The wallpaper.</value>
    public byte[] Wallpaper
    {
        get { return GetValue<byte[]>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Parameter property value.
    /// </summary>
    /// <value>The tag.</value>
    public object Parameter
    {
        get { return GetValue<object>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Height property value.
    /// </summary>
    /// <value>The height.</value>
    public double Height
    {
        get { return GetValue<double>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Width property value.
    /// </summary>
    /// <value>The width.</value>
    public double Width
    {
        get { return GetValue<double>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Color property value.
    /// </summary>
    /// <value>The color.</value>
    public string Color
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }
}
