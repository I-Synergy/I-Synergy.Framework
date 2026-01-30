using Dotmim.Sync;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace ISynergy.Framework.Synchronization.Extensions;

public static class SyncSetupExtensions
{
    public static SyncSetup WithTenantFilter(this SyncSetup setup, Type[] entities)
    {
        foreach (var entity in entities.EnsureNotNull())
        {
            var tableName = entity.Name;

            if (entity.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault() is TableAttribute attribute)
                tableName = attribute.Name;

            if (!setup.Tables.Any(q => q.TableName.Equals(tableName)))
                setup.Tables.Add(tableName);

            if (entity.GetProperty(GenericConstants.TenantId) is not null)
            {
                var filter = new SetupFilter(tableName);
                filter.AddParameter(GenericConstants.TenantId, DbType.Guid, false);
                filter.AddWhere(GenericConstants.TenantId, tableName, GenericConstants.TenantId);
                setup.Filters.Add(filter);
            }
        }

        return setup;
    }
}
