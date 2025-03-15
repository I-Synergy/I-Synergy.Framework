using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.EntityFramework.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Reflection;

namespace ISynergy.Framework.EntityFramework.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies the decimal precision.
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <param name="currencyPrecision"></param>
    /// <returns></returns>
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
    /// Applies the version to entity.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <summary>
    /// Applies the query filters.
    /// </summary>
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
    /// Applies the query filters.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="tenantId"></param>
    /// <summary>
    /// Applies the query filters.
    /// </summary>
    public static ModelBuilder ApplyTenantFilters(this ModelBuilder modelBuilder, Func<Guid> tenantId)
    {
        var clrTypes = modelBuilder.Model.GetEntityTypes().Select(et => et.ClrType).ToList();

        // Apply tenantFilter 
        foreach (var type in clrTypes.Where(t => typeof(ITenantEntity).IsAssignableFrom(t)).EnsureNotNull())
        {
            // Create a properly typed parameter for the specific entity type
            var parameter = Expression.Parameter(type, "e");
            var tenantIdProperty = Expression.Property(parameter, nameof(ITenantEntity.TenantId));
            var tenantIdCall = Expression.Call(Expression.Constant(tenantId.Target), tenantId.Method);
            var equalExpression = Expression.Equal(tenantIdProperty, tenantIdCall);
            var tenantFilter = Expression.Lambda(equalExpression, parameter);

            var existingFilter = modelBuilder.Model.FindEntityType(type).GetQueryFilter();
            if (existingFilter is null)
            {
                // Directly apply the tenant filter if no existing filter
                modelBuilder.Entity(type).HasQueryFilter(tenantFilter);
            }
            else
            {
                // Combine with existing filter only if necessary
                modelBuilder.Entity(type)
                    .HasQueryFilter(CombineQueryFilters(type, new[] { existingFilter, tenantFilter }));
            }
        }

        return modelBuilder;
    }

    /// <summary>
    /// Applies the soft delete query filters.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <summary>
    /// Applies the query filters.
    /// </summary>
    public static ModelBuilder ApplySoftDeleteFilters(this ModelBuilder modelBuilder)
    {
        var clrTypes = modelBuilder.Model.GetEntityTypes()
            .Select(et => et.ClrType)
            .Where(t => typeof(IEntity).IsAssignableFrom(t) && !t.GetCustomAttributes(typeof(IgnoreSoftDeleteAttribute), true).Any())
            .ToList();

        // Apply softDeleteFilter 
        foreach (var type in clrTypes.Where(t => typeof(IEntity).IsAssignableFrom(t)).EnsureNotNull())
        {
            // Create a properly typed parameter for the specific entity type
            var parameter = Expression.Parameter(type, "e");
            var isDeletedProperty = Expression.Property(parameter, nameof(IEntity.IsDeleted));
            var notExpression = Expression.Not(isDeletedProperty);
            var softDeleteFilter = Expression.Lambda(notExpression, parameter);

            var existingFilter = modelBuilder.Model.FindEntityType(type).GetQueryFilter();

            if (existingFilter is null)
            {
                // Directly apply the soft delete filter if no existing filter
                modelBuilder.Entity(type).HasQueryFilter(softDeleteFilter);
            }
            else
            {
                // Combine with existing filter only if necessary
                modelBuilder.Entity(type)
                    .HasQueryFilter(CombineQueryFilters(type, new[] { existingFilter, softDeleteFilter }));
            }
        }

        return modelBuilder;
    }

    // This is an expansion on the limitation in EFCore as described in ConvertFilterExpression<T>.
    // Since EFCore currently only allows 1 HasQueryFilter() call (and ignores all previous calls),
    // we need to create a single lambda expression for all filters.
    // See: https://github.com/aspnet/EntityFrameworkCore/issues/10275
    /// <summary>
    /// Combines the query filters.
    /// </summary>
    /// <param name="entityType">Type of the entity.</param>
    /// <param name="expressions">The and also expressions.</param>
    /// <returns>LambdaExpression.</returns>
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

    public static ModelBuilder ApplyModelBuilderConfigurations(this ModelBuilder modelBuilder, Assembly[] assemblies)
    {
        // Get the open generic type for IEntityTypeConfiguration
        var configurationInterface = typeof(IEntityTypeConfiguration<>);

        // Filter assemblies that contain classes implementing IEntityTypeConfiguration<>
        var entityAssemblies = assemblies
            .Where(a => !a.IsDynamic &&
                        a.GetTypes().Any(t =>
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
}
