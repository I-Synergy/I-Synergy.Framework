using SQLite;

namespace ISynergy.Framework.AspNetCore.WebDav.FileSystem.SQLite
{
    [Table("filesystementrydata")]
    internal class FileData
    {
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }

        [Column("data")]
        public byte[] Data { get; set; }
    }
}
