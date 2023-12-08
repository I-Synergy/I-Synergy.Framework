namespace ISynergy.Framework.Core.Performance.Models;

public class BasicModel
{
    public Guid Id { get; }
    public string Name { get; }
    public int Age { get; }

    public BasicModel()
    {
        Id = Guid.NewGuid();
        Name = $"Name {Id}";
        Age = new Random().Next();
    }
}
