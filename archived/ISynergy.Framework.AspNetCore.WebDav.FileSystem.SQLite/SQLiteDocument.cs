using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;
using SQLitePCL;

namespace ISynergy.Framework.AspNetCore.WebDav.FileSystem.SQLite
{
    /// <summary>
    /// A <see cref="SQLitePCL"/> based implementation of a WebDAV document
    /// </summary>
    internal class SQLiteDocument : SQLiteEntry, IDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDocument"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system this document belongs to</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="info">The file information</param>
        /// <param name="path">The root-relative path of this document</param>
        public SQLiteDocument(SQLiteFileSystem fileSystem, ICollection parent, FileEntry info, Uri path)
            : base(fileSystem, parent, info, path, null)
        {
        }

        /// <inheritdoc />
        public long Length => Info.Length;

        /// <inheritdoc />
        public Task<Stream> OpenReadAsync(CancellationToken cancellationToken)
        {
            var result = Connection.CreateCommand("select rowid from filesystementrydata where id=?", Info.Id)
                .ExecuteQuery<RowIdTemp>()
                .FirstOrDefault();
            if (result == null)
                return Task.FromResult<Stream>(new MemoryStream());

            sqlite3_blob blob;
            var rc = raw.sqlite3_blob_open(
                Connection.Handle,
                "main",
                "filesystementrydata",
                "data",
                Convert.ToInt64(result.RowId),
                0,
                out blob);
            if (rc != 0)
                throw new SQLiteFileSystemException(Connection.Handle);

            var stream = new SQLiteBlobReadStream(Connection.Handle, blob);
            return Task.FromResult<Stream>(stream);
        }

        /// <inheritdoc />
        public Task<Stream> CreateAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<Stream>(new SQLiteBlobWriteStream(Connection, Info));
        }

        /// <inheritdoc />
        public override async Task<DeleteResult> DeleteAsync(CancellationToken cancellationToken)
        {
            Connection.RunInTransaction(() =>
            {
                Connection.Delete<FileData>(Info.Id);
                Connection.Delete(Info);
            });

            var propStore = FileSystem.PropertyStore;
            if (propStore != null)
            {
                await propStore.RemoveAsync(this, cancellationToken);
            }

            return new DeleteResult(WebDavStatusCode.OK, null);
        }

        /// <inheritdoc />
        public async Task<IDocument> CopyToAsync(ICollection collection, string name, CancellationToken cancellationToken)
        {
            var targetId = collection.Path.Append(name, false).OriginalString.ToLowerInvariant();
            var dir = (SQLiteCollection)collection;
            var targetEntry = new FileEntry()
            {
                Id = targetId,
                Name = name,
                Path = collection.Path.OriginalString,
                CreationTimeUtc = Info.CreationTimeUtc,
                LastWriteTimeUtc = Info.LastWriteTimeUtc,
                ETag = Info.ETag,
                Length = Info.Length,
            };

            Connection.RunInTransaction(() =>
            {
                Connection.InsertOrReplace(targetEntry);
                Connection
                    .CreateCommand(
                        "insert or replace into filesystementrydata (id, data) select ?, src.data from filesystementrydata src where src.id=?",
                        targetId,
                        Info.Id)
                    .ExecuteNonQuery();
            });

            var doc = new SQLiteDocument(dir.SQLiteFileSystem, dir, targetEntry, dir.Path.Append(name, false));

            var sourcePropStore = FileSystem.PropertyStore;
            var destPropStore = collection.FileSystem.PropertyStore;
            if (sourcePropStore != null && destPropStore != null)
            {
                var sourceProps = await sourcePropStore.GetAsync(this, cancellationToken);
                await destPropStore.RemoveAsync(doc, cancellationToken);
                await destPropStore.SetAsync(doc, sourceProps, cancellationToken);
            }
            else if (destPropStore != null)
            {
                await destPropStore.RemoveAsync(doc, cancellationToken);
            }

            return doc;
        }

        /// <inheritdoc />
        public async Task<IDocument> MoveToAsync(ICollection collection, string name, CancellationToken cancellationToken)
        {
            var newDoc = await CopyToAsync(collection, name, cancellationToken);
            await DeleteAsync(cancellationToken);
            return newDoc;
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class RowIdTemp
        {
            public long RowId
            {
                get;
                set;
            }
        }
    }
}
