namespace ISynergy.Framework.AspNetCore.Models;

public class SwaggerEndpoint
{
    public Uri Endpoint { get; set; }
    public string Description { get; set; }
    public Version Version { get; set; }
}
