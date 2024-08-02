using ISynergy.Framework.Core.Base;
using System.ComponentModel;

namespace Sample.Models;

/// <summary>
/// Class MediaItem.
/// </summary>
[Bindable(BindableSupport.Yes)]
public class MediaItem : BaseModel
{
    /// <summary>
    /// Gets or sets the Index property value.
    /// </summary>
    /// <value>The index.</value>
    public int Index
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the ImageUri property value.
    /// </summary>
    /// <value>The image URI.</value>
    public string ImageUri
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }
}
