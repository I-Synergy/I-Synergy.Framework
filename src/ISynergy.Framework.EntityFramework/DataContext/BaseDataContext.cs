using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.EntityFramework.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;


namespace ISynergy.Framework.EntityFramework.DataContext;

/// <summary>
/// Class BaseDataContext.
/// Implements the <see cref="DbContext" />
/// </summary>
/// <seealso cref="DbContext" />
public abstract class BaseDataContext : DbContext
{
    /// <summary>
    /// The join prefix
    /// </summary>
    protected const string JoinPrefix = "Join_";
    /// <summary>
    /// The currency precision
    /// </summary>
    protected const string CurrencyPrecision = "decimal(38, 10)";

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDataContext"/> class.
    /// </summary>
    protected BaseDataContext()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDataContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    protected BaseDataContext(DbContextOptions options)
        : base(options)
    {
    }

    protected virtual void ApplyDecimalPrecision(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var decimalProperties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal));

            foreach (var property in decimalProperties)
            {
                modelBuilder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasColumnType(CurrencyPrecision);
            }
        }
    }

    /// <summary>
    /// Applies the query filters.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="tenantId"></param>
    /// <summary>
    /// Applies the query filters.
    /// </summary>
    protected virtual void ApplyQueryFilters(ModelBuilder modelBuilder, Func<Guid> tenantId)
    {
        var clrTypes = modelBuilder.Model.GetEntityTypes().Select(et => et.ClrType).ToList();

        var tenantFilter = (Expression<Func<BaseTenantEntity, bool>>)(e => e.TenantId == tenantId.Invoke());
        var softDeleteFilter = (Expression<Func<BaseClass, bool>>)(e => !e.IsDeleted);

        // Apply tenantFilter and softDeleteFilter 
        foreach (var type in clrTypes.Where(t => typeof(BaseTenantEntity).IsAssignableFrom(t)).EnsureNotNull())
        {
            var filter = CombineQueryFilters(type, new LambdaExpression[] { tenantFilter, softDeleteFilter });
            modelBuilder.Entity(type).HasQueryFilter(filter);
        }

        // Apply default version
        foreach (var type in clrTypes.Where(t => typeof(IClass).IsAssignableFrom(t)).EnsureNotNull())
        {
            modelBuilder.Entity(type)
                .Property<int>(nameof(IClass.Version))
                .HasDefaultValue(1);
        }
    }

    /// <summary>
    /// Called when [before saving].
    /// </summary>
    protected virtual void OnBeforeSaving(Func<Guid> tenantId, Func<string> username)
    {
        if (!ChangeTracker.HasChanges())
            return;

        foreach (var entry in ChangeTracker.Entries<BaseTenantEntity>().Where(e => e.State == EntityState.Added).EnsureNotNull())
        {
            entry.Entity.TenantId = tenantId.Invoke();

            if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                entry.Entity.CreatedBy = username.Invoke();

            entry.Entity.CreatedDate = DateTimeOffset.UtcNow;
            entry.Entity.Version = 1;
        }

        foreach (var entry in ChangeTracker.Entries<BaseTenantEntity>().Where(e => e.State == EntityState.Modified).EnsureNotNull())
        {
            if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                entry.Entity.CreatedBy = username.Invoke();

            if (entry.Entity.CreatedDate == DateTimeOffset.MinValue)
                entry.Entity.CreatedDate = DateTimeOffset.UtcNow;

            if (string.IsNullOrEmpty(entry.Entity.ChangedBy))
                entry.Entity.ChangedBy = username.Invoke();

            entry.Entity.ChangedDate = DateTimeOffset.UtcNow;
            entry.Entity.Version = entry.Entity.Version + 1;
        }
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
    protected static LambdaExpression CombineQueryFilters(Type entityType, IEnumerable<LambdaExpression> andAlsoExpressions)
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
