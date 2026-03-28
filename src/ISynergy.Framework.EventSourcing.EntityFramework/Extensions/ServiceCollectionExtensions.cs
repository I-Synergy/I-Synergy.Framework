using ISynergy.Framework.EventSourcing.Abstractions.Aggregates;
using ISynergy.Framework.EventSourcing.Abstractions.Events;
using ISynergy.Framework.EventSourcing.Aggregates;
using ISynergy.Framework.EventSourcing.EntityFramework.Abstractions;
using ISynergy.Framework.EventSourcing.EntityFramework.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Extensions;

/// <summary>
/// Extension methods for registering Event Sourcing EF Core services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="EventSourcingDbContext"/>, <see cref="EventStore"/>,
    /// <see cref="DefaultEventTypeResolver"/>, and <see cref="JsonReflectionEventSerializer"/>
    /// with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="optionsAction">
    /// Configures the EF Core provider (e.g. <c>UseSqlServer</c>, <c>UseNpgsql</c>).
    /// </param>
    /// <returns>The <paramref name="services"/> for chaining.</returns>
    /// <remarks>
    /// <para>
    /// This overload uses reflection-based JSON serialization and assembly scanning for type
    /// resolution. It is not AOT/trim-safe. For AOT-safe registration use
    /// <see cref="AddEventSourcingEntityFramework(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action{Microsoft.EntityFrameworkCore.DbContextOptionsBuilder}, System.Text.Json.Serialization.JsonSerializerContext, System.Collections.Generic.IReadOnlyDictionary{string, System.Type})"/>
    /// instead.
    /// </para>
    /// <para>
    /// Registers <see cref="EventSourcingDbContext"/> as scoped via <c>AddDbContext</c>.
    /// Do NOT replace this with <c>AddDbContextPool</c> — the global tenant query filter
    /// captures <c>ITenantService</c> by reference and pooling would share the closure
    /// across scopes.
    /// </para>
    /// <para>
    /// Prerequisite: <c>ITenantService</c> must be registered before calling this method
    /// (e.g. via <c>services.AddMultiTenancyIntegration()</c> from
    /// <c>ISynergy.Framework.AspNetCore.MultiTenancy</c>).
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("Uses DefaultEventTypeResolver which scans AppDomain assemblies, and JsonReflectionEventSerializer which uses reflection-based JSON. Use the AOT-safe overload for trim/AOT scenarios.")]
    [RequiresDynamicCode("Uses JsonReflectionEventSerializer which requires dynamic code generation. Use the AOT-safe overload for trim/AOT scenarios.")]
    public static IServiceCollection AddEventSourcingEntityFramework(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction)
    {
        services.AddDbContext<EventSourcingDbContext>(optionsAction);
        services.AddScoped<IEventStore, EventStore>();
        services.AddSingleton<IEventTypeResolver, DefaultEventTypeResolver>();
        services.AddSingleton<IEventSerializer, JsonReflectionEventSerializer>();
        return services;
    }

    /// <summary>
    /// Registers the <see cref="EventSourcingDbContext"/>, <see cref="EventStore"/>,
    /// <see cref="DictionaryEventTypeResolver"/>, and <see cref="JsonSourceGeneratedEventSerializer"/>
    /// with the dependency injection container. This overload is AOT/trim-safe.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="optionsAction">
    /// Configures the EF Core provider (e.g. <c>UseSqlServer</c>, <c>UseNpgsql</c>).
    /// </param>
    /// <param name="jsonSerializerContext">
    /// A source-generated <see cref="JsonSerializerContext"/> with all event types declared
    /// via <c>[JsonSerializable(typeof(YourEventType))]</c>.
    /// </param>
    /// <param name="eventTypeMap">
    /// An explicit mapping from stored event type names to CLR types. Keys must match
    /// the short type name (<c>nameof(YourEventType)</c>) used by
    /// <see cref="DictionaryEventTypeResolver.GetTypeName"/>.
    /// </param>
    /// <returns>The <paramref name="services"/> for chaining.</returns>
    /// <example>
    /// <code>
    /// [JsonSerializable(typeof(OrderPlaced))]
    /// [JsonSerializable(typeof(OrderShipped))]
    /// public partial class OrderEventJsonContext : JsonSerializerContext { }
    ///
    /// services.AddEventSourcingEntityFramework(
    ///     o => o.UseNpgsql(connectionString),
    ///     jsonSerializerContext: OrderEventJsonContext.Default,
    ///     eventTypeMap: new Dictionary&lt;string, Type&gt;
    ///     {
    ///         [nameof(OrderPlaced)]  = typeof(OrderPlaced),
    ///         [nameof(OrderShipped)] = typeof(OrderShipped),
    ///     });
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// JSON serialization and event type resolution are AOT/trim-safe when using this overload.
    /// However, EF Core's model building still requires either a compiled model
    /// (<c>optionsAction</c> calling <c>UseModel(CompiledModel)</c>) or suppression of
    /// <c>IL2026</c>/<c>IL3050</c> if full AOT publishing is not required.
    /// See <see href="https://aka.ms/efcore-docs-trimming"/> for details.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("EF Core model building requires unreferenced code unless a compiled model is used. See https://aka.ms/efcore-docs-trimming.")]
    [RequiresDynamicCode("EF Core model building requires dynamic code unless a compiled model is used.")]
    public static IServiceCollection AddEventSourcingEntityFramework(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction,
        JsonSerializerContext jsonSerializerContext,
        IReadOnlyDictionary<string, Type> eventTypeMap)
    {
        services.AddDbContext<EventSourcingDbContext>(optionsAction);
        services.AddScoped<IEventStore, EventStore>();
        services.AddSingleton<IEventTypeResolver>(new DictionaryEventTypeResolver(eventTypeMap));
        services.AddSingleton<IEventSerializer>(new JsonSourceGeneratedEventSerializer(jsonSerializerContext));
        return services;
    }

    /// <summary>
    /// Registers <see cref="AggregateRepository{TAggregate, TId}"/> as the
    /// <see cref="IAggregateRepository{TAggregate, TId}"/> implementation for the given aggregate.
    /// </summary>
    /// <typeparam name="TAggregate">The aggregate root type.</typeparam>
    /// <typeparam name="TId">
    /// The aggregate identifier type. Must be a <see cref="Guid"/> value type at runtime.
    /// </typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The <paramref name="services"/> for chaining.</returns>
    /// <example>
    /// <code>
    /// services
    ///     .AddEventSourcingEntityFramework(o => o.UseSqlServer(connectionString))
    ///     .AddAggregateRepository&lt;OrderAggregate, Guid&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddAggregateRepository<TAggregate, TId>(
        this IServiceCollection services)
        where TAggregate : AggregateRoot<TId>, new()
        where TId : struct
    {
        services.AddScoped<IAggregateRepository<TAggregate, TId>, AggregateRepository<TAggregate, TId>>();
        return services;
    }
}
