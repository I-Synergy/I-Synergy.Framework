using Dotmim.Sync;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using System.Data;

namespace ISynergy.Framework.Synchronization.Extensions;
public static class SyncSetupExtensions
{
    public static SyncSetup WithTenantFilter(this SyncSetup setup)
    {
        Argument.IsNotNull(setup);

        foreach (var table in setup.Tables.EnsureNotNull())
        {
            var filter = new SetupFilter(table.TableName);
            filter.AddParameter(nameof(IBaseTenantEntity.TenantId), DbType.Guid, false);
            filter.AddWhere(nameof(IBaseTenantEntity.TenantId), table.TableName, nameof(IBaseTenantEntity.TenantId));
            setup.Filters.Add(filter);
        }

        return setup;
    }
}
