using ISynergy.Framework.Synchronization.Core.Database;
using System;
using System.Data.Common;

namespace ISynergy.Framework.Synchronization.Core.Arguments
{
    public class UpgradeProgressArgs : ProgressArgs
    {
        private string message;

        public SyncTable Table { get; }

        public Version Version { get; }

        public UpgradeProgressArgs(SyncContext context, string message, Version version, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            this.message = message;
            Version = version;
        }

        public override string Source => Connection.Database;
        public override string Message => message;

        public override int EventId => 999999;
    }
}
