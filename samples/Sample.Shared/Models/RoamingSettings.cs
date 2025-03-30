using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Base;

namespace Sample.Models;

public class RoamingSettings : ObservableClass, IRoamingSettings
{
    /// <summary>
    /// Gets or sets the SynchronizationSetting property value.
    /// </summary>
    public SynchronizationSettings? SynchronizationSetting
    {
        get => GetValue<SynchronizationSettings>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the IsSynchronizationEnabled property value.
    /// </summary>
    public bool IsSynchronizationEnabled
    {
        get => GetValue<bool>();
        set
        {
            SetValue(value);

            if (value && SynchronizationSetting is null)
                SynchronizationSetting = new SynchronizationSettings();
            else if (!value)
                SynchronizationSetting = null;
        }
    }

    public RoamingSettings()
    {
        IsSynchronizationEnabled = false;
        SynchronizationSetting = default;
    }
}
