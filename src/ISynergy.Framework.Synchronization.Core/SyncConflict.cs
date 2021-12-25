using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Set;

namespace ISynergy.Framework.Synchronization.Core
{
    /// <summary>
    /// Represents a synchronization conflict at the row level.
    /// Conflict rule resolution is set on the server side
    /// </summary>
    public class SyncConflict
    {

        /// <summary>
        /// Gets or sets the error message that is returned when a conflict is set to ConflictType.ErrorsOccurred
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the row that contains the conflicting row from the local database.
        /// </summary>
        public SyncRow LocalRow { get; set; }

        /// <summary>
        /// Gets the row that contains the conflicting row from the remote database.
        /// </summary>
        public SyncRow RemoteRow { get; set; }

        /// <summary>
        /// Gets or sets the ConflictType enumeration value that represents the type of synchronization conflict.
        /// </summary>
        public ConflictType Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the SyncConflict class by using default values.
        /// </summary>
        public SyncConflict()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SyncConflict class by using conflict type and conflict stage parameters.
        /// </summary>
        public SyncConflict(ConflictType type)
        {
            Type = type;
        }


        /// <summary>
        /// add a local row
        /// </summary>
        internal void AddLocalRow(SyncRow row) => LocalRow = row;

        /// <summary>
        /// add a remote row
        /// </summary>
        internal void AddRemoteRow(SyncRow row) => RemoteRow = row;
    }
}
