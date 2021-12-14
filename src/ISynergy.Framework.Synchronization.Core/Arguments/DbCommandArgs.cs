using ISynergy.Framework.Synchronization.Core.Enumerations;
using System.Data.Common;

namespace ISynergy.Framework.Synchronization.Core.Arguments
{
    internal class DbCommandArgs : ProgressArgs
    {
        public DbCommandArgs(SyncContext context, DbCommand command, DbConnection connection, DbTransaction transaction)
            : base(context, connection, transaction)
        {
            Command = command;
        }
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Sql;

        public override string Source => Connection.Database;
        public override string Message => $"[{Connection.Database}] Sql Statement:{Command.CommandText}.";

        public override int EventId => SyncEventsId.ConnectionOpen.Id;

        public DbCommand Command { get; }
    }
}
