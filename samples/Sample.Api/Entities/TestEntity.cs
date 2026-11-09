using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sample.Api.Entities;

[Serializable]
public class TestEntity
{
    [Key]
    [JsonRequired]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [JsonRequired]
    public DateTime CreatedDate { get; set; }
}
