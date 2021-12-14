namespace ISynergy.Framework.Synchronization.Core.Enumerations
{
    /// <summary>
    /// Defines logging severity levels.
    /// </summary>    
    public enum SyncProgressLevel
    {
        /// <summary>
        /// Progress that contain the most detailed messages and the Sql statement executed
        /// These messages may contain sensitive application data. 
        /// These messages are disabled by default and should never be enabled in a production environment.
        /// </summary>
        Sql,
        /// <summary>
        /// Progress that contain the most detailed messages. 
        /// These messages may contain sensitive application data. 
        /// These messages are disabled by default and should never be enabled in a production environment.
        /// </summary>        
        Trace,
        /// <summary>
        /// Progress that are used for interactive investigation during development.
        /// These logs should primarily contain information useful for debugging and have no long-term value.
        /// </summary>
        Debug,
        /// <summary>
        /// Progress that track the general flow of the application. These logs should have long-term value.
        /// </summary>
        Information,
        /// <summary>
        /// Progress that highlight an abnormal or unexpected event in the application flow,
        /// but do not otherwise cause the application execution to stop.
        /// </summary>
        Warning,
        /// <summary>
        /// Progress that highlight when the current flow of execution is stopped due to a failure.
        /// These should indicate a failure in the current activity, not an application-wide failure.
        /// </summary>
        Error,
        /// <summary>
        /// Not used for writing progress messages. Specifies that a logging category should not write any messages.
        /// </summary>
        None
    }
}
