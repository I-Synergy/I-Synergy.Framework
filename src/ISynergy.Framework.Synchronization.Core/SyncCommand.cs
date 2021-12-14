namespace ISynergy.Framework.Synchronization.Core
{
    public class SyncCommand
    {
        //public DbCommand DbCommand { get; set; }

        public bool IsPrepared { get; set; }
        public string CommandCodeName { get; }

        public SyncCommand(string commandCodeName)
        {
            CommandCodeName = commandCodeName;
            IsPrepared = false;

        }
        //public SyncCommand(DbCommand dbCommand)
        //{
        //    DbCommand = dbCommand;
        //    IsPrepared = false;
        //}
    }
}
