using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Performance.Models
{
    public class EntityModel : EntityBase
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Age { get; }

        public EntityModel()
        {
            Id = Guid.NewGuid();
            Name = $"Name {Id}";
            Age = new Random().Next();
        }
    }
}
