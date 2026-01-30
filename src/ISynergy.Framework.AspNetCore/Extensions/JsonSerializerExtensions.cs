using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace ISynergy.Framework.AspNetCore.Extensions;

/// <summary>
/// Extension methods for JSON serialization configuration
/// </summary>
/// <summary>
/// Extension methods for JSON serialization configuration
/// </summary>
public static class JsonSerializerExtensions
{
    /// <summary>
    /// Applies the default JSON serializer options from DefaultJsonSerializers.Web() to the provided JsonSerializerOptions
    /// </summary>
    /// <param name="options">The JsonSerializerOptions to configure</param>
    public static void ApplyDefaultJsonSerializerOptions(this JsonSerializerOptions options)
    {
        // Get the default serializer options
        var defaultOptions = DefaultJsonSerializers.Web;

        // Copy all properties and settings from the default options
        options.PropertyNamingPolicy = defaultOptions.PropertyNamingPolicy;
        options.DefaultIgnoreCondition = defaultOptions.DefaultIgnoreCondition;
        options.PropertyNameCaseInsensitive = defaultOptions.PropertyNameCaseInsensitive;
        options.NumberHandling = defaultOptions.NumberHandling;
        options.DictionaryKeyPolicy = defaultOptions.DictionaryKeyPolicy;
        options.WriteIndented = defaultOptions.WriteIndented;
        options.ReadCommentHandling = defaultOptions.ReadCommentHandling;
        options.ReferenceHandler = defaultOptions.ReferenceHandler;

        // Clear existing converters and add all converters from the default options
        options.Converters.Clear();

        foreach (var converter in defaultOptions.Converters.EnsureNotNull())
        {
            options.Converters.Add(converter);
        }
    }

    /// <summary>
    /// Applies the default JSON serializer options to MVC JsonOptions
    /// </summary>
    /// <param name="options">The MVC JsonOptions to configure</param>
    public static void ApplyDefaultJsonSerializerOptions(this Microsoft.AspNetCore.Mvc.JsonOptions options)
    {
        options.JsonSerializerOptions.ApplyDefaultJsonSerializerOptions();
    }

    /// <summary>
    /// Applies the default JSON serializer options to Minimal API JsonOptions
    /// </summary>
    /// <param name="options">The Minimal API JsonOptions to configure</param>
    public static void ApplyDefaultJsonSerializerOptions(this Microsoft.AspNetCore.Http.Json.JsonOptions options)
    {
        options.SerializerOptions.ApplyDefaultJsonSerializerOptions();
    }

    /// <summary>
    /// Configures all JSON serialization options in the service collection to use the default settings
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <param name="includeControllers">Include registrations for mvc and controllers</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection ConfigureDefaultJsonSerialization(this IServiceCollection services, bool includeControllers = false)
    {
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
                options.ApplyDefaultJsonSerializerOptions());

        return services;
    }

    /// <summary>
    /// Configures all JSON serialization options in the service collection to use the default settings
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <returns>The service collection for chaining</returns>
    public static IMvcBuilder AddControllerWithDefaultJsonSerialization(this IServiceCollection services) =>
        AddControllerWithDefaultJsonSerialization(services, null);

    /// <summary>
    /// Configures all JSON serialization options in the service collection to use the default settings
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <param name="configure"></param>
    /// <returns>The service collection for chaining</returns>
    public static IMvcBuilder AddControllerWithDefaultJsonSerialization(this IServiceCollection services, Action<MvcOptions>? configure)
    {
        services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options => options.ApplyDefaultJsonSerializerOptions());

        var mvcBuilder = services.AddControllers(configure)
            .AddJsonOptions(options => options.ApplyDefaultJsonSerializerOptions());

        return mvcBuilder;
    }
}
