using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Performance.Models;

public class ObservableModel : ObservableValidatedClass
{
    /// <summary>
    /// Gets or sets the Id property value.
    /// </summary>
    public Guid Id
    {
        get => GetValue<Guid>();
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Name property value.
    /// </summary>
    public string Name
    {
        get => GetValue<string>();
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Age property value.
    /// </summary>
    public int Age
    {
        get => GetValue<int>();
        private set => SetValue(value);
    }

    public ObservableModel()
    {
        Id = Guid.NewGuid();
        Name = $"Name {Id}";
        Age = new Random().Next();
    }
}
