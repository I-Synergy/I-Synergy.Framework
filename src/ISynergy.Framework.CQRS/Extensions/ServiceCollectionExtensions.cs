using ISynergy.Framework.CQRS.Abstractions.Commands;
using ISynergy.Framework.CQRS.Abstractions.Dispatchers;
using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.Decorators;
using ISynergy.Framework.CQRS.Dispatchers;
using ISynergy.Framework.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ISynergy.Framework.CQRS.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CQRS services to the specified IServiceCollection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddCQRS(this IServiceCollection services)
    {
        // Add dispatchers
        services.TryAddScoped<ICommandDispatcher, CommandDispatcher>();
        services.TryAddScoped<IQueryDispatcher, QueryDispatcher>();

        return services;
    }

    /// <summary>
    /// Registers all command and query handlers from specified assemblies
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">Assemblies to scan</param>
    /// <returns>Service collection for chaining</returns>
    [RequiresUnreferencedCode("Scans assemblies with GetExportedTypes()/GetInterfaces() to find ICommandHandler/IQueryHandler implementors; not trim-compatible. Types not otherwise referenced by the application may already have been removed by the trimmer.")]
    public static IServiceCollection AddHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        RegisterCommandHandlers(services, assemblies);
        RegisterQueryHandlers(services, assemblies);

        return services;
    }

    /// <summary>
    /// Adds notification decorators to all command handlers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>Service collection for chaining</returns>
    /// <remarks>
    /// Not NativeAOT-compatible. Builds decorator types with MakeGenericType for whatever
    /// ICommandHandler&lt;,&gt; instantiations happen to be registered at runtime -- an
    /// instantiation not seen at compile time throws under NativeAOT. There is currently no
    /// AOT-safe alternative for this method (would require a source generator that doesn't
    /// exist yet in this repo); avoid calling it from a NativeAOT-published app.
    /// </remarks>
    [RequiresDynamicCode("Uses MakeGenericType to construct NotificationCommandHandlerDecorator<> instantiations at runtime; not supported under NativeAOT.")]
    [RequiresUnreferencedCode("Reflects over registered ICommandHandler<> service descriptors; not trim-compatible.")]
    public static IServiceCollection AddCommandNotifications(this IServiceCollection services)
    {
        // Get all registered command handler types
        var serviceDescriptors = services
            .Where(descriptor =>
                descriptor.ServiceType.IsGenericType &&
                (descriptor.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                 descriptor.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
            .ToList();

        foreach (var descriptor in serviceDescriptors) // NOSONAR
        {
            var serviceType = descriptor.ServiceType;

            if (serviceType.IsGenericType)
            {
                var genericArgs = serviceType.GetGenericArguments();

                if (genericArgs.Length == 1)
                {
                    // This is ICommandHandler<TCommand>
                    var decoratorType = typeof(NotificationCommandHandlerDecorator<>).MakeGenericType(genericArgs);
                    services.Decorate(serviceType, decoratorType);
                }
                else if (genericArgs.Length == 2)
                {
                    // This is ICommandHandler<TCommand, TResult>
                    var decoratorType = typeof(NotificationCommandHandlerDecorator<,>).MakeGenericType(genericArgs);
                    services.Decorate(serviceType, decoratorType);
                }
            }
        }

        return services;
    }

    /// <summary>
    /// Adds logging decorators to all command and query handlers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>Service collection for chaining</returns>
    /// <remarks>
    /// Not NativeAOT-compatible. Builds decorator types with MakeGenericType for whatever
    /// ICommandHandler&lt;&gt; instantiations happen to be registered at runtime -- an
    /// instantiation not seen at compile time throws under NativeAOT. There is currently no
    /// AOT-safe alternative for this method (would require a source generator that doesn't
    /// exist yet in this repo); avoid calling it from a NativeAOT-published app.
    /// </remarks>
    [RequiresDynamicCode("Uses MakeGenericType to construct LoggingCommandHandlerDecorator<> instantiations at runtime; not supported under NativeAOT.")]
    [RequiresUnreferencedCode("Reflects over registered ICommandHandler<> service descriptors; not trim-compatible.")]
    public static IServiceCollection AddCQRSLogging(this IServiceCollection services)
    {
        // Get all registered command handler types
        var commandHandlerDescriptors = services
            .Where(descriptor =>
                descriptor.ServiceType.IsGenericType &&
                descriptor.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
            .ToList();

        foreach (var descriptor in commandHandlerDescriptors) // NOSONAR
        {
            var serviceType = descriptor.ServiceType;
            var genericArgs = serviceType.GetGenericArguments();
            var decoratorType = typeof(LoggingCommandHandlerDecorator<>).MakeGenericType(genericArgs);
            services.Decorate(serviceType, decoratorType);
        }

        return services;
    }

    // Required for decorator pattern - helper extension method
    private static IServiceCollection Decorate<TService, TDecorator>(this IServiceCollection services)
        where TDecorator : TService
    {
        return services.Decorate(typeof(TService), typeof(TDecorator));
    }

    private static IServiceCollection Decorate(this IServiceCollection services, Type serviceType, Type decoratorType)
    {
        var descriptors = services.Where(s => s.ServiceType == serviceType).ToList();

        if (descriptors.Count == 0)
        {
            throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered.");
        }

        foreach (var descriptor in descriptors) // NOSONAR
        {
            var index = services.IndexOf(descriptor);
            services.Remove(descriptor);

            var decoratorDescriptor = new ServiceDescriptor(
                serviceType,
                sp =>
                {
                    var service = GetInstance(sp, descriptor);
                    return ActivatorUtilities.CreateInstance(sp, decoratorType, service);
                },
                descriptor.Lifetime);

            services.Insert(index, decoratorDescriptor);
        }

        return services;
    }

    private static object GetInstance(IServiceProvider serviceProvider, ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationInstance != null)
            return descriptor.ImplementationInstance;

        if (descriptor.ImplementationFactory != null)
            return descriptor.ImplementationFactory(serviceProvider);

        if (descriptor.ImplementationType == null)
            throw new InvalidOperationException("ServiceDescriptor must have an ImplementationType, ImplementationFactory, or ImplementationInstance.");

        return ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, descriptor.ImplementationType);
    }

    private static void RegisterCommandHandlers(IServiceCollection services, Assembly[] assemblies)
    {
        // Find all command handler implementations
        var handlerTypes = assemblies
            .SelectMany(a => a.GetExportedTypes())
            .Where(t => t.GetInterfaces().Any(i => IsCommandHandlerInterface(i)))
            .ToList();

        foreach (var handlerType in handlerTypes) // NOSONAR
        {
            var handlerInterfaces = handlerType.GetInterfaces().Where(IsCommandHandlerInterface);

            foreach (var handlerInterface in handlerInterfaces) // NOSONAR
            {
                services.AddScoped(handlerInterface, handlerType);
            }
        }
    }

    private static void RegisterQueryHandlers(IServiceCollection services, Assembly[] assemblies)
    {
        // Find all query handler implementations
        var handlerTypes = assemblies
            .SelectMany(a => a.GetExportedTypes())
            .Where(t => t.GetInterfaces().Any(i => IsQueryHandlerInterface(i)))
            .ToList();

        foreach (var handlerType in handlerTypes) // NOSONAR
        {
            var handlerInterfaces = handlerType.GetInterfaces().Where(IsQueryHandlerInterface);

            foreach (var handlerInterface in handlerInterfaces) // NOSONAR
            {
                services.AddScoped(handlerInterface, handlerType);
            }
        }
    }

    private static bool IsCommandHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var typeDefinition = type.GetGenericTypeDefinition();

        return typeDefinition == typeof(ICommandHandler<>) ||
               typeDefinition == typeof(ICommandHandler<,>);
    }

    private static bool IsQueryHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var typeDefinition = type.GetGenericTypeDefinition();

        return typeDefinition == typeof(IQueryHandler<,>);
    }
}