using ISynergy.Framework.Core.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sample.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
            .AddMvc()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                options.JsonSerializerOptions.AllowTrailingCommas = true;
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
                options.JsonSerializerOptions.ReferenceHandler = null;

                options.JsonSerializerOptions.Converters.Add(new IsoDateTimeJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new IsoDateTimeOffsetJsonConverter());
            });

        builder.Services.AddControllersWithViews();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.UseOpenApi();
        app.UseSwaggerUi();

        //app.UseOpenApi();
        //app.UseReDoc();

        app.Run();
    }
}
