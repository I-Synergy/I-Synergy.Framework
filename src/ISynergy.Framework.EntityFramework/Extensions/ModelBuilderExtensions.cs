using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.EntityFramework.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

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

        var tenantFilter = (Expression<Func<BaseTenantEntity, bool>>)(e => e.TenantId == tenantId.Invoke());

        // Apply tenantFilter 
        foreach (var type in clrTypes.Where(t => typeof(BaseTenantEntity).IsAssignableFrom(t)).EnsureNotNull())
        {
            var filter = CombineQueryFilters(type, new LambdaExpression[] { tenantFilter });
            modelBuilder.Entity(type).HasQueryFilter(filter);
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
        var clrTypes = modelBuilder.Model.GetEntityTypes().Select(et => et.ClrType).ToList();
        var softDeleteFilter = (Expression<Func<BaseClass, bool>>)(e => !e.IsDeleted);

        // Apply softDeleteFilter 
        foreach (var type in clrTypes.Where(t => typeof(BaseEntity).IsAssignableFrom(t)).EnsureNotNull())
        {
            var filter = CombineQueryFilters(type, new LambdaExpression[] { softDeleteFilter });
            modelBuilder.Entity(type).HasQueryFilter(filter);
        }

        return modelBuilder;
    }

    /// <summary>
    /// Applies the version query filters.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <summary>
    /// Applies the query filters.
    /// </summary>
    public static ModelBuilder ApplyVersionFilters(this ModelBuilder modelBuilder)
    {
        var clrTypes = modelBuilder.Model.GetEntityTypes().Select(et => et.ClrType).ToList();

        // Apply default version
        foreach (var type in clrTypes.Where(t => typeof(IClass).IsAssignableFrom(t)).EnsureNotNull())
        {
            modelBuilder.Entity(type)
                .Property<int>(nameof(IClass.Version))
                .HasDefaultValue(1);
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
    /// <param name="andAlsoExpressions">The and also expressions.</param>
    /// <returns>LambdaExpression.</returns>
    public static LambdaExpression CombineQueryFilters(Type entityType, IEnumerable<LambdaExpression> andAlsoExpressions)
    {
        var newParam = Expression.Parameter(entityType);

        var andAlsoExprBase = (Expression<Func<BaseTenantEntity, bool>>)(_ => true);
        var andAlsoExpr = ReplacingExpressionVisitor.Replace(andAlsoExprBase.Parameters.Single(), newParam, andAlsoExprBase.Body);

        foreach (var expressionBase in andAlsoExpressions.EnsureNotNull())
        {
            var expression = ReplacingExpressionVisitor.Replace(expressionBase.Parameters.Single(), newParam, expressionBase.Body);
            andAlsoExpr = Expression.AndAlso(andAlsoExpr, expression);
        }

        return Expression.Lambda(andAlsoExpr, newParam);
    }
}
