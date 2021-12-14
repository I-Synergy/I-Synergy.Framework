using ISynergy.Framework.Synchronization.Core.Enumerations;
using System.Data.Common;

namespace ISynergy.Framework.Synchronization.Core.Arguments
{
    public class ProgressArgs
    {
        /// <summary>
        /// Current connection used 
        /// </summary>
        public DbConnection Connection { get; internal set; }

        /// <summary>
        /// Current transaction used for the sync
        /// </summary>
        public DbTransaction Transaction { get; internal set; }

        /// <summary>
        /// Gets the current context
        /// </summary>
        public SyncContext Context { get; }

        /// <summary>
        /// Gets the Progress Level
        /// </summary>
        public virtual SyncProgressLevel ProgressLevel { get; }

        /// <summary>
        /// Gets or Sets an arbitrary args you can use for you own purpose
        /// </summary>
        public virtual string Hint { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        public ProgressArgs(SyncContext context, DbConnection connection, DbTransaction transaction)
        {
            Context = context;
            Connection = connection;
            Transaction = transaction;
            Message = GetType().Name;
            ProgressLevel = SyncProgressLevel.Information;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="connection"></param>
        public ProgressArgs(SyncContext context, DbConnection connection)
        {
            Context = context;
            Connection = connection;
            Message = GetType().Name;
            ProgressLevel = SyncProgressLevel.Information;
        }


        /// <summary>
        /// Gets the args type
        /// </summary>
        public string TypeName => GetType().Name;

        /// <summary>
        /// return a global message about current progress
        /// </summary>
        public virtual string Message { get; } = string.Empty;

        /// <summary>
        /// return the progress initiator source
        /// </summary>
        public virtual string Source { get; } = string.Empty;

        /// <summary>
        /// Gets the event id, used for logging purpose
        /// </summary>
        public virtual int EventId { get; } = 1;

        /// <summary>
        /// Gets the overall percentage progress
        /// </summary>
        public double ProgressPercentage => Context.ProgressPercentage;

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Message))
                return Message;

            return base.ToString();
        }



    }
}
