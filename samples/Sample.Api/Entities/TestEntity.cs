using System.ComponentModel.DataAnnotations;

namespace Sample.Api.Entities;

public class TestEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
}
