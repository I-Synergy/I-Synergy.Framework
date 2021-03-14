using System;
using System.IO;
using System.Linq;
using ISynergy.Framework.Core.Tests.Fixtures;
using Xunit;

namespace ISynergy.Framework.Core.IO.Tests
{
    /// <summary>
    /// Class FileWatcherTests.
    /// Implements the <see cref="IClassFixture{FileWatcherFixture}" />
    /// </summary>
    /// <seealso cref="IClassFixture{FileWatcherFixture}" />
    public class FileWatcherTests : IClassFixture<FileWatcherFixture>
    {
        /// <summary>
        /// The fixture
        /// </summary>
        private readonly FileWatcherFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcherTests"/> class.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        public FileWatcherTests(FileWatcherFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Files the watcher added test.
        /// </summary>
        public void FileWatcherAddedTest()
        {
            var count = 1000;

            var location = AppDomain.CurrentDomain.BaseDirectory;
            var folder = Directory.CreateDirectory(Path.Combine(location, nameof(FileWatcherAddedTest))).FullName;

            if (_fixture.InitializeWatcher(folder, false))
            {
                _fixture.FileWatcher.Start();

                for (int i = 0; i < count; i++)
                {
                    File.WriteAllText(Path.Combine(folder, Path.GetRandomFileName()), $"File: {Path.GetRandomFileName()}");
                }

                _fixture.FileWatcher.Stop();
                _fixture.RemoveEventHandlers();

                Directory.GetFiles(folder).ToList().ForEach(File.Delete);
            }

            Assert.NotEmpty(_fixture.ObservedFiles);
            Assert.Equal(count, _fixture.ObservedFiles.Where(q => q.EventName == "Created").Count());
        }

        /// <summary>
        /// Files the watcher deleted test.
        /// </summary>
        public void FileWatcherDeletedTest()
        {
            var count = 1000;

            var location = AppDomain.CurrentDomain.BaseDirectory;
            var folder = Directory.CreateDirectory(Path.Combine(location, nameof(FileWatcherDeletedTest))).FullName;

            if (_fixture.InitializeWatcher(folder, false))
            {
                _fixture.FileWatcher.Start();

                for (int i = 0; i < count; i++)
                {
                    File.WriteAllText(Path.Combine(folder, Path.GetRandomFileName()), $"File: {Path.GetRandomFileName()}");
                }

                Directory.GetFiles(folder).ToList().ForEach(File.Delete);

                _fixture.FileWatcher.Stop();
                _fixture.RemoveEventHandlers();
            }

            Assert.NotEmpty(_fixture.ObservedFiles);
            Assert.Equal(count, _fixture.ObservedFiles.Where(q => q.EventName == "Deleted").Count());
        }
    }
}
