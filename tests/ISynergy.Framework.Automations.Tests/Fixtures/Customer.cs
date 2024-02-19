using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Automations.Tests.Fixtures;

public class Customer : ModelBase
{
    public event EventHandler Registered;

    /// <summary>
    /// Gets or sets the Name property value.
    /// </summary>
    public string Name
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Email property value.
    /// </summary>
    public string Email
    {
        get { return GetValue<string>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Age property value.
    /// </summary>
    public int Age
    {
        get { return GetValue<int>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the Active property value.
    /// </summary>
    public bool Active
    {
        get { return GetValue<bool>(); }
        set { SetValue(value); }
    }

    public enum ProcessState
    {
        Unregistered,
        Registered,
        Active,
        Blocked
    }

    public List<string> Actions { get; set; } = ["Block", "Activate", "Add", "Delete"];

    public void Register()
    {
        Registered?.Invoke(this, EventArgs.Empty);
    }
}
