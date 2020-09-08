﻿using System;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model.Headers;
using SQLite;

namespace ISynergy.Framework.AspNetCore.WebDav.FileSystem.SQLite
{
    [Table("filesystementries")]
    internal class FileEntry
    {
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }

        [Indexed]
        [Column("path")]
        public string Path { get; set; }

        [Indexed("name_type", 0)]
        [Column("name")]
        public string Name { get; set; }

        [Indexed("name_type", 1)]
        [Column("collection")]
        public bool IsCollection { get; set; }

        [Column("mtime")]
        public DateTime LastWriteTimeUtc { get; set; } = DateTime.UtcNow;

        [Column("ctime")]
        public DateTime CreationTimeUtc { get; set; } = DateTime.UtcNow;

        [Column("length")]
        public long Length { get; set; }

        [Column("etag")]
        public string ETag { get; set; } = new EntityTag(false).ToString();
    }
}
