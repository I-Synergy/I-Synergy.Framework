using Dotmim.Sync;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Synchronization.Extensions;

/// <summary>
/// Extension methods for <see cref="SyncSetup"/> to simplify tenant-filtered synchronization configuration.
/// </summary>
public static class SyncSetupExtensions
{
    /// <summary>
    /// Configures tenant ID filters on all supplied entity types and adds them to the <see cref="SyncSetup"/> table list.
    /// </summary>
    /// <param name="setup">The <see cref="SyncSetup"/> instance to configure.</param>
    /// <param name="entities">
    /// An array of entity <see cref="Type"/> values whose <c>TenantId</c> properties should be
    /// filtered. Each type is inspected for a <see cref="System.ComponentModel.DataAnnotations.Schema.TableAttribute"/>
    /// to resolve the table name; if none is present the type name is used.
    /// </param>
    /// <returns>The configured <see cref="SyncSetup"/> instance.</returns>
    /// <remarks>
    /// <para>
    /// This method uses <c>Type.GetCustomAttributes(Type, bool)</c> and
    /// <c>Type.GetProperty(string)</c> to inspect the caller-supplied <paramref name="entities"/>
    /// at runtime. Because the element types are not statically known at compile time, the linker
    /// cannot guarantee that <see cref="System.ComponentModel.DataAnnotations.Schema.TableAttribute"/>
    /// metadata or the <c>TenantId</c> property descriptor will be preserved on those types in
    /// trimmed builds.
    /// </para>
    /// <para>
    /// For AOT-safe builds, ensure that the entity types passed to this method are preserved by
    /// adding <c>[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]</c> to the
    /// parameter array elements, or by supplying a linker root descriptor that retains those types.
    /// </para>
    /// <para>
    /// Additionally, <c>Dotmim.Sync</c> itself is not AOT-compatible as of early 2026.
    /// Applications publishing with <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c> should not
    /// use <c>ISynergy.Framework.Synchronization</c> until <c>Dotmim.Sync</c> gains AOT support.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode(
        "WithTenantFilter uses reflection to read TableAttribute and property names from runtime types. " +
        "Ensure entity types and their attributes are preserved in trimmed builds.")]
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
