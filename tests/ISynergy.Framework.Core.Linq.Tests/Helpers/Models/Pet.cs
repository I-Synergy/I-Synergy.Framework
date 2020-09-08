
namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Person Owner { get; set; }
        public int OwnerId { get; set; }
        public int? NullableOwnerId { get; set; }
    }
}
