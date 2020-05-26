using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace ISynergy.Framework.EntityFramework.DataContext
{
    public abstract class BaseDataContext : DbContext
    {
        protected const string JoinPrefix = "Join_";
        protected const string CurrencyPrecision = "decimal(38, 10)";

        private readonly ITenantService _tenantService;

        protected BaseDataContext(DbContextOptions options, ITenantService tenantService)
            : base(options)
        {
            _tenantService = tenantService;
        }

        public virtual void ApplyQueryFilters(ModelBuilder modelBuilder)
        {
            var clrTypes = modelBuilder.Model.GetEntityTypes().Select(et => et.ClrType).ToList();

            var tenantFilter = (Expression<Func<BaseTenantEntity, bool>>)(e => e.TenantId == _tenantService.TenantId);

            // Apply tenantFilter
            foreach (var type in clrTypes.Where(t => typeof(BaseTenantEntity).IsAssignableFrom(t)).EnsureNotNull())
            {
                var filter = CombineQueryFilters(type, new LambdaExpression[] { tenantFilter });
                modelBuilder.Entity(type).HasQueryFilter(filter);
            }

            // Apply default version
            foreach (var type in clrTypes.Where(t => typeof(IClassBase).IsAssignableFrom(t)).EnsureNotNull())
            {
                modelBuilder.Entity(type)
                    .Property<int>(nameof(IClassBase.Version))
                    .HasDefaultValue(1);
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            if (ChangeTracker.HasChanges())
            {
                foreach (var entry in ChangeTracker.Entries<BaseTenantEntity>().Where(e => e.State == EntityState.Added).EnsureNotNull())
                {
                    entry.Entity.TenantId = _tenantService.TenantId;

                    if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                        entry.Entity.CreatedBy = _tenantService.UserName;

                    entry.Entity.CreatedDate = DateTimeOffset.UtcNow;
                    entry.Entity.Version = 1;
                }

                foreach (var entry in ChangeTracker.Entries<BaseTenantEntity>().Where(e => e.State == EntityState.Modified).EnsureNotNull())
                {
                    if (string.IsNullOrEmpty(entry.Entity.ChangedBy))
                        entry.Entity.ChangedBy = _tenantService.UserName;

                    entry.Entity.ChangedDate = DateTimeOffset.UtcNow;
                    entry.Entity.Version = entry.Entity.Version + 1;
                }
            }
        }

        // This is an expansion on the limitation in EFCore as described in ConvertFilterExpression<T>.
        // Since EFCore currently only allows 1 HasQueryFilter() call (and ignores all previous calls),
        // we need to create a single lambda expression for all filters.
        // See: https://github.com/aspnet/EntityFrameworkCore/issues/10275
        private static LambdaExpression CombineQueryFilters(Type entityType, IEnumerable<LambdaExpression> andAlsoExpressions)
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

        // This is necessary due to a current limitation in EFCore.
        // SetQueryFilter() only accepts an expression of a data entity type, not a base class or interface.
        // Here we rewrite the expression from a base class / interface to a data entity type.
        // This can be removed if this issue is resolved:
        // https://github.com/aspnet/EntityFrameworkCore/issues/10257
        private static LambdaExpression ConvertFilterExpression<T>(
            Expression<Func<T, bool>> filterExpression,
            Type entityType)
        {
            var newParam = Expression.Parameter(entityType);
            var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam, filterExpression.Body);

            return Expression.Lambda(newBody, newParam);
        }
    }
}
