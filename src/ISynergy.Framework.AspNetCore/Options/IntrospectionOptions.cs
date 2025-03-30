namespace ISynergy.Framework.AspNetCore.Options;
public class IntrospectionOptions
{
    public required string Issuer { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}
