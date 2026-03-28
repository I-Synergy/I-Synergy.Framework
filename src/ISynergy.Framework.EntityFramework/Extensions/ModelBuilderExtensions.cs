using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.EntityFramework.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace ISynergy.Framework.EntityFramework.Extensions;

/// <summary>
/// Extension methods for <see cref="ModelBuilder"/> that configure EF Core models.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies decimal column precision to all decimal properties in the model.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="currencyPrecision">The SQL column type string for decimal precision. Defaults to <c>decimal(38, 10)</c>.</param>
    /// <returns>The <paramref name="modelBuilder"/> for chaining.</returns>
    /// <remarks>
    /// This method calls <see cref="Type.GetProperties()"/> at runtime and is not AOT-safe.
    /// Applications publishing with Native AOT should use EF Core compiled models
    /// (<c>dotnet ef dbcontext optimize</c>) and register them via <c>UseModel()</c> in
    /// <c>OnConfiguring</c> to avoid runtime model-building reflection.
    /// </remarks>
    [RequiresUnreferencedCode("EF Core model building uses reflection. Use compiled models for AOT.")]
    [RequiresDynamicCode("Builds LINQ expression trees at runtime.")]
    public static ModelBuilder ApplyDecimalPrecision(this ModelBuilder modelBuilder, string currencyPrecision = "decimal(38, 10)")
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var decimalProperties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal));

            foreach (var property in decimalProperties)
            {
                modelBuilder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasColumnType(currencyPrecision);
            }
        }

        return modelBuilder;
    }

    /// <summary>
    /// Applies EF Core row-version concurrency tokens to all entity types that implement
    /// <see cref="IEntity"/> and are not decorated with <see cref="IgnoreVersioningAttribute"/>.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>The <paramref name="modelBuilder"/> for chaining.</returns>
    /// <remarks>
    /// This method calls <c>Type.GetCustomAttributes(Type, bool)</c> at runtime and is not AOT-safe.
    /// Applications publishing with Native AOT should use EF Core compiled models
    /// (<c>dotnet ef dbcontext optimize</c>) and register them via <c>UseModel()</c> in
    /// <c>OnConfiguring</c> to avoid runtime model-building reflection.
    /// </remarks>
    [RequiresUnreferencedCode("EF Core model building uses reflection. Use compiled models for AOT.")]
    [RequiresDynamicCode("Builds LINQ expression trees at runtime.")]
    public static ModelBuilder ApplyVersioning(this ModelBuilder modelBuilder)
    {
        var clrTypes = modelBuilder.Model.GetEntityTypes()
            .Select(et => et.ClrType)
            .Where(t => typeof(IEntity).IsAssignableFrom(t) && !t.GetCustomAttributes(typeof(IgnoreVersioningAttribute), true).Any())
            .ToList();

        // Apply default version
        foreach (var type in clrTypes.Where(t => typeof(IClass).IsAssignableFrom(t)).EnsureNotNull())
        {
            modelBuilder.Entity(type)
                .Property<int>(nameof(IClass.Version))
                .HasDefaultValue(1);
        }

        return modelBuilder;
    }

    /// <summary>
    /// Applies a global query filter scoping all <see cref="ITenantEntity"/> entities to the current tenant.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="tenantId">A delegate that returns the current tenant identifier at query execution time.</param>
    /// <returns>The <paramref name="modelBuilder"/> for chaining.</returns>
    /// <remarks>
    /// This method constructs LINQ expression trees at runtime using <see cref="Expression.Lambda(Expression,ParameterExpression[])"/>
    /// and is not AOT-safe. Applications publishing with Native AOT should define query filters directly
    /// in <c>OnModelCreating</c> using the strongly-typed
    /// <c>HasQueryFilter&lt;T&gt;(Expression&lt;Func&lt;T, bool&gt;&gt;)</c> overload instead of calling this helper.
    /// </remarks>
    [RequiresUnreferencedCode("EF Core model building uses reflection. Use compiled models for AOT.")]
    [RequiresDynamicCode("Builds LINQ expression trees at runtime.")]
    public static ModelBuilder ApplyTenantFilters(this ModelBuilder modelBuilder, Func<Guid> tenantId)
    {
        var clrTypes = modelBuilder.Model.GetEntityTypes().Select(et => et.ClrType).ToList();

        foreach (var type in clrTypes.Where(t => typeof(ITenantEntity).IsAssignableFrom(t)).EnsureNotNull())
        {
            var parameter = Expression.Parameter(type, "e");
            var tenantIdProperty = Expression.Property(parameter, nameof(ITenantEntity.TenantId));
            var tenantIdCall = Expression.Call(Expression.Constant(tenantId.Target), tenantId.Method);
            var equalExpression = Expression.Equal(tenantIdProperty, tenantIdCall);
            var tenantFilter = Expression.Lambda(equalExpression, parameter);
            var entityType = modelBuilder.Model.FindEntityType(type);

            if (entityType is null)
                continue;

            var existingFilters = entityType.GetDeclaredQueryFilters();
            var existingFilter = existingFilters?.FirstOrDefault();

            if (existingFilter is null)
            {
                modelBuilder.Entity(type).HasQueryFilter(tenantFilter);
            }
            else
            {
                // Filter out nulls to satisfy the non-nullable IEnumerable<LambdaExpression> parameter
                var filters = new[] { (LambdaExpression?)existingFilter.Expression, tenantFilter }
                    .Where(f => f is not null)!
                    .Cast<LambdaExpression>();

                var combinedFilter = CombineQueryFilters(type, filters);

                modelBuilder.Entity(type).HasQueryFilter(combinedFilter);
            }
        }

        return modelBuilder;
    }

    /// <summary>
    /// Applies a global soft-delete query filter to all entity types that implement <see cref="IEntity"/>
    /// and are not decorated with <see cref="IgnoreSoftDeleteAttribute"/>.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>The <paramref name="modelBuilder"/> for chaining.</returns>
    /// <remarks>
    /// This method constructs LINQ expression trees at runtime using <see cref="Expression.Lambda(Expression,ParameterExpression[])"/>
    /// and is not AOT-safe. Applications publishing with Native AOT should define query filters directly
    /// in <c>OnModelCreating</c> using the strongly-typed
    /// <c>HasQueryFilter&lt;T&gt;(Expression&lt;Func&lt;T, bool&gt;&gt;)</c> overload instead of calling this helper.
    /// </remarks>
    [RequiresUnreferencedCode("EF Core model building uses reflection. Use compiled models for AOT.")]
    [RequiresDynamicCode("Builds LINQ expression trees at runtime.")]
    public static ModelBuilder ApplySoftDeleteFilters(this ModelBuilder modelBuilder)
    {
        var clrTypes = modelBuilder.Model.GetEntityTypes()
            .Select(et => et.ClrType)
            .Where(t => typeof(IEntity).IsAssignableFrom(t) && !t.GetCustomAttributes(typeof(IgnoreSoftDeleteAttribute), true).Any())
            .ToList();

        foreach (var type in clrTypes.Where(t => typeof(IEntity).IsAssignableFrom(t)).EnsureNotNull())
        {
            var parameter = Expression.Parameter(type, "e");
            var isDeletedProperty = Expression.Property(parameter, nameof(IEntity.IsDeleted));
            var notExpression = Expression.Not(isDeletedProperty);
            var softDeleteFilter = Expression.Lambda(notExpression, parameter);
            var entityType = modelBuilder.Model.FindEntityType(type);

            if (entityType is null)
                continue;

            var existingFilters = entityType.GetDeclaredQueryFilters();
            var existingFilter = existingFilters?.FirstOrDefault();

            if (existingFilter is null)
            {
                modelBuilder.Entity(type).HasQueryFilter(softDeleteFilter);
            }
            else
            {
                // Filter out nulls to satisfy the non-nullable IEnumerable<LambdaExpression> parameter
                var filters = new[] { (LambdaExpression?)existingFilter.Expression, softDeleteFilter }
                    .Where(f => f is not null)!
                    .Cast<LambdaExpression>();

                var combinedFilter = CombineQueryFilters(type, filters);

                modelBuilder.Entity(type).HasQueryFilter(combinedFilter);
            }
        }

        return modelBuilder;
    }

    // This is an expansion on the limitation in EFCore as described in ConvertFilterExpression<T>.
    // Since EFCore currently only allows 1 HasQueryFilter() call (and ignores all previous calls),
    // we need to create a single lambda expression for all filters.
    // See: https://github.com/aspnet/EntityFrameworkCore/issues/10275
    /// <summary>
    /// Combines multiple query filter lambda expressions into a single <c>AndAlso</c> expression.
    /// </summary>
    /// <param name="entityType">The CLR type of the entity the filter applies to.</param>
    /// <param name="expressions">The lambda expressions to combine.</param>
    /// <returns>A single <see cref="LambdaExpression"/> that is the logical AND of all supplied expressions.</returns>
    /// <remarks>
    /// This method constructs LINQ expression trees at runtime and is not AOT-safe.
    /// </remarks>
    [RequiresUnreferencedCode("EF Core model building uses reflection. Use compiled models for AOT.")]
    [RequiresDynamicCode("Builds LINQ expression trees at runtime.")]
    public static LambdaExpression CombineQueryFilters(Type entityType, IEnumerable<LambdaExpression> expressions)
    {
        var parameter = Expression.Parameter(entityType);

        // Get the expressions list and ensure it's not null
        var filterExpressions = expressions.EnsureNotNull().ToList();

        if (!filterExpressions.Any())
            return Expression.Lambda(Expression.Constant(true), parameter);

        // Start with the first expression
        var firstExpr = filterExpressions[0];
        var combinedExpr = ReplacingExpressionVisitor.Replace(
            firstExpr.Parameters.Single(),
            parameter,
            firstExpr.Body);

        // Combine the rest of the expressions with AndAlso
        for (int i = 1; i < filterExpressions.Count; i++)
        {
            var expression = ReplacingExpressionVisitor.Replace(
                filterExpressions[i].Parameters.Single(),
                parameter,
                filterExpressions[i].Body);

            combinedExpr = Expression.AndAlso(combinedExpr, expression);
        }

        return Expression.Lambda(combinedExpr, parameter);
    }

    /// <summary>
    /// Applies all <see cref="IEntityTypeConfiguration{TEntity}"/> implementations found in the supplied
    /// assemblies to the <paramref name="modelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="assemblies">The assemblies to scan for configuration types.</param>
    /// <returns>The <paramref name="modelBuilder"/> for chaining.</returns>
    /// <remarks>
    /// This overload scans assemblies at runtime using <see cref="Assembly.GetExportedTypes()"/>, which is
    /// an AOT blocker — the linker cannot determine which types are exported from unknown assemblies at trim time.
    /// Prefer the <see cref="ApplyModelBuilderConfigurations(ModelBuilder, IReadOnlyList{Type})"/> overload
    /// that accepts an explicit list of configuration types for AOT/trimming compatibility.
    /// </remarks>
    [Obsolete("Prefer the overload accepting IReadOnlyList<Type> for AOT/trimming compatibility. This overload uses assembly scanning which is not AOT-safe.")]
    [RequiresUnreferencedCode("Assembly scanning is not AOT-compatible. Use the IReadOnlyList<Type> overload instead.")]
    [RequiresDynamicCode("Assembly scanning uses reflection.")]
    public static ModelBuilder ApplyModelBuilderConfigurations(this ModelBuilder modelBuilder, Assembly[] assemblies)
    {
        // Get the open generic type for IEntityTypeConfiguration
        var configurationInterface = typeof(IEntityTypeConfiguration<>);

        // Filter assemblies that contain classes implementing IEntityTypeConfiguration<>
        var entityAssemblies = assemblies
            .Where(a => !a.IsDynamic &&
                        a.GetExportedTypes().Any(t =>
                            t.IsClass &&
                            !t.IsAbstract &&
                            t.GetInterfaces().Any(i =>
                                i.IsGenericType &&
                                i.GetGenericTypeDefinition() == configurationInterface)))
            .ToList();

        // Apply configurations from each relevant assembly
        foreach (var assembly in entityAssemblies.EnsureNotNull())
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

        return modelBuilder;
    }

    /// <summary>
    /// Applies the supplied <see cref="IEntityTypeConfiguration{TEntity}"/> implementation types to the
    /// <paramref name="modelBuilder"/> without performing any assembly scanning.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="configurationTypes">
    /// An explicit list of types that implement <see cref="IEntityTypeConfiguration{TEntity}"/>.
    /// Each type must have a public parameterless constructor.
    /// </param>
    /// <returns>The <paramref name="modelBuilder"/> for chaining.</returns>
    /// <remarks>
    /// <para>
    /// This overload is the AOT-preferred alternative to <see cref="ApplyModelBuilderConfigurations(ModelBuilder, Assembly[])"/>.
    /// Because the type list is supplied by the caller, the linker can statically analyse which types are used.
    /// </para>
    /// <para>
    /// <c>Activator.CreateInstance</c> is still used internally, so the method requires
    /// <c>RequiresDynamicCode</c> — but the assembly-scanning blocker is eliminated.
    /// </para>
    /// <example>
    /// <code>
    /// modelBuilder.ApplyModelBuilderConfigurations(new List&lt;Type&gt;
    /// {
    ///     typeof(UserEntityConfiguration),
    ///     typeof(OrderEntityConfiguration),
    /// });
    /// </code>
    /// </example>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when a type in <paramref name="configurationTypes"/> does not implement
    /// <see cref="IEntityTypeConfiguration{TEntity}"/> or cannot be instantiated.
    /// </exception>
    [RequiresUnreferencedCode("Uses Type.GetInterfaces() and Activator.CreateInstance with runtime-resolved types. Not AOT-safe.")]
    [RequiresDynamicCode("Uses Activator.CreateInstance to instantiate configuration types at runtime.")]
    public static ModelBuilder ApplyModelBuilderConfigurations(this ModelBuilder modelBuilder, IReadOnlyList<Type> configurationTypes)
    {
        var configurationInterface = typeof(IEntityTypeConfiguration<>);

        foreach (var type in configurationTypes.EnsureNotNull())
        {
            if (!type.IsClass || type.IsAbstract)
                throw new ArgumentException($"Type '{type.FullName}' must be a non-abstract class.", nameof(configurationTypes));

            var implementsInterface = type.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == configurationInterface);

            if (!implementsInterface)
                throw new ArgumentException($"Type '{type.FullName}' does not implement IEntityTypeConfiguration<>.", nameof(configurationTypes));

            var instance = Activator.CreateInstance(type)
                ?? throw new ArgumentException($"Could not create an instance of type '{type.FullName}'.", nameof(configurationTypes));

            modelBuilder.ApplyConfiguration((dynamic)instance);
        }

        return modelBuilder;
    }
}
