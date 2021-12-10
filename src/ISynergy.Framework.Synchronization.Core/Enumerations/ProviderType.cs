using System;

namespace ISynergy.Framework.Synchronization.Core.Enumerations
{
    [Flags]
    public enum ProviderType
    {
        Sql = 0x1,
        MySql = 0x2,
        Sqlite = 0x40,
        MariaDB = 0x80,
    }
}
