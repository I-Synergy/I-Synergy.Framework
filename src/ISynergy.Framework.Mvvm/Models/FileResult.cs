using System;
using System.IO;

namespace ISynergy.Framework.Mvvm.Models
{
    /// <summary>
    /// Class FileResult.
    /// </summary>
    public class FileResult : IDisposable
    {
        /// <summary>
        /// Backing store for the FileName property
        /// </summary>
        private string _fileName;

        /// <summary>
        /// Backing store for the FilePath property
        /// </summary>
        private string _filePath;

        /// <summary>
        /// Function to get a stream to the picked file.
        /// </summary>
        private readonly Func<Stream> _streamGetter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileResult"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileTypeId">The file type identifier.</param>
        /// <param name="streamGetter">The stream getter.</param>
        /// <param name="dispose">The dispose.</param>
        public FileResult(string filePath, string fileName, int fileTypeId, Func<Stream> streamGetter, Action<bool> dispose = null)
        {
            _filePath = filePath;
            _fileName = fileName;
            _dispose = dispose;
            _streamGetter = streamGetter;

            FileTypeId = fileTypeId;
        }

        /// <summary>
        /// Gets or sets the file type identifier.
        /// </summary>
        /// <value>The file type identifier.</value>
        public int FileTypeId { get; private set; }

        /// <summary>
        /// Filename of the picked file, without path
        /// </summary>
        /// <value>The name of the file.</value>
        /// <exception cref="System.ObjectDisposedException">null</exception>
        /// <exception cref="System.ObjectDisposedException">null</exception>
        public string FileName
        {
            get
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(null);

                return _fileName;
            }

            set
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(null);

                _fileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        public byte[] File
        {
            get
            {
                using var stream = GetStream();
                return ReadFully(stream);
            }
        }

        /// <summary>
        /// Gets stream to access the picked file.
        /// Note that when DataArray property was already accessed, the stream
        /// must be rewinded to the beginning.
        /// </summary>
        /// <returns>stream object</returns>
        /// <exception cref="System.ObjectDisposedException">null</exception>
        public Stream GetStream()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(null);

            return _streamGetter();
        }

        /// <summary>
        /// Full filepath of the picked file.
        /// Note that on specific platforms this can also contain an URI that
        /// can't be opened with file related APIs. Use DataArray property or
        /// GetStream() method in this cases.
        /// </summary>
        /// <value>The file path.</value>
        /// <exception cref="System.ObjectDisposedException">null</exception>
        /// <exception cref="System.ObjectDisposedException">null</exception>
        public string FilePath
        {
            get
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(null);

                return _filePath;
            }

            set
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(null);

                _filePath = value;
            }
        }

        /// <summary>
        /// Completely reads all bytes from the input stream and returns it as byte array. Can be
        /// used when the returned file data consists of a stream, not a real filename.
        /// </summary>
        /// <param name="input">input stream</param>
        /// <returns>byte array</returns>
        internal static byte[] ReadFully(Stream input)
        {
            using var ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }

        #region IDispose implementation
        /// <summary>
        /// Indicates if the object is already disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Action to dispose of underlying resources of the picked file.
        /// </summary>
        private readonly Action<bool> _dispose;

        /// <summary>
        /// Disposes of all resources in the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of managed resources
        /// </summary>
        /// <param name="disposing">True when called from Dispose(), false when called from the destructor</param>
        private void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _dispose?.Invoke(disposing);
        }

        /// <summary>
        /// Finalizer for this object
        /// </summary>
        ~FileResult()
        {
            Dispose(false);
        }
        #endregion
    }
}
