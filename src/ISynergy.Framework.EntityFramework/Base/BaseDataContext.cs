namespace ISynergy.Framework.EntityFramework.Base
{
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
        /// The tenant service
        /// </summary>
        private readonly ITenantService _tenantService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDataContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="tenantService">The tenant service.</param>
        protected BaseDataContext(DbContextOptions options, ITenantService tenantService)
            : base(options)
        {
            _tenantService = tenantService;
        }

        /// <summary>
        /// Applies the query filters.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
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

        /// <summary>
        /// <para>
        /// Saves all changes made in this context to the database.
        /// </para>
        /// <para>
        /// This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        /// changes to entity instances before saving to the underlying database. This can be disabled via
        /// <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        /// </para>
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Indicates whether <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges" /> is called after the changes have
        /// been sent successfully to the database.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// <para>
        /// Saves all changes made in this context to the database.
        /// </para>
        /// <para>
        /// This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        /// changes to entity instances before saving to the underlying database. This can be disabled via
        /// <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        /// </para>
        /// <para>
        /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        /// that any asynchronous operations have completed before calling another method on this context.
        /// </para>
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Indicates whether <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges" /> is called after the changes have
        /// been sent successfully to the database.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the
        /// number of state entries written to the database.</returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Called when [before saving].
        /// </summary>
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
        /// <summary>
        /// Combines the query filters.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="andAlsoExpressions">The and also expressions.</param>
        /// <returns>LambdaExpression.</returns>
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
        /// <summary>
        /// Converts the filter expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterExpression">The filter expression.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>LambdaExpression.</returns>
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
