using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using System;
using System.Data.Common;

namespace ISynergy.Framework.Synchronization.Core.Arguments
{
    public class UpgradeProgressArgs : ProgressArgs
    {
        private string _message;

        public SyncTable Table { get; }

        public Version Version { get; }

        public UpgradeProgressArgs(SyncContext context, string message, Version version, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            _message = message;
            Version = version;
        }

        public override string Source => Connection.Database;
        public override string Message => _message;
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Information;
        public override int EventId => 999999;
    }
}
