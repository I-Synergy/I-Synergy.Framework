﻿using System;
using System.IO;
using System.Linq;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model.Headers;
using SQLite;

namespace ISynergy.Framework.AspNetCore.WebDav.FileSystem.SQLite
{
    internal class SQLiteBlobWriteStream : Stream
    {
        private readonly SQLiteConnection _connection;
        private readonly FileEntry _entry;
        private readonly MemoryStream _baseStream = new MemoryStream();

        public SQLiteBlobWriteStream(SQLiteConnection connection, FileEntry entry)
        {
            _connection = connection;
            _entry = entry;
        }

        /// <inheritdoc />
        public override bool CanRead { get; } = false;

        /// <inheritdoc />
        public override bool CanSeek { get; } = true;

        /// <inheritdoc />
        public override bool CanWrite { get; } = true;

        /// <inheritdoc />
        public override long Length => _baseStream.Length;

        /// <inheritdoc />
        public override long Position
        {
            get { return _baseStream.Position; }
            set { _baseStream.Position = value; }
        }

        /// <inheritdoc />
        public override void Flush()
        {
            SaveData();
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek(offset, origin);
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SaveData();
            }

            base.Dispose(disposing);
        }

        private void SaveData()
        {
            _connection.RunInTransaction(() =>
            {
                var oldLength = _entry.Length;
                var oldModificationTime = _entry.LastWriteTimeUtc;
                var oldETag = _entry.ETag;
                try
                {
                    var data = new FileData()
                    {
                        Id = _entry.Id,
                        Data = _baseStream.ToArray(),
                    };
                    _entry.Length = data.Data.Length;
                    _entry.LastWriteTimeUtc = DateTime.UtcNow;
                    _entry.ETag = EntityTag.Parse(_entry.ETag).Single().Update().ToString();

                    _connection.InsertOrReplace(data);
                    _connection.Update(_entry);
                }
                catch (Exception)
                {
                    _entry.Length = oldLength;
                    _entry.LastWriteTimeUtc = oldModificationTime;
                    _entry.ETag = oldETag;
                    throw;
                }
            });
        }
    }
}
