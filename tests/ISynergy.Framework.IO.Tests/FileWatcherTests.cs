using ISynergy.Framework.IO.Tests.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace ISynergy.Framework.IO.Tests
{
    /// <summary>
    /// Class FileWatcherTests.
    /// </summary>
    public class FileWatcherTests
    {
        /// <summary>
        /// The fixture
        /// </summary>
        private readonly FileWatcherFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcherTests"/> class.
        /// </summary>
        public FileWatcherTests()
        {
            _fixture = new FileWatcherFixture();
        }

        /// <summary>
        /// Files the watcher added test.
        /// </summary>
        public void FileWatcherAddedTest()
        {
            int count = 1000;

            string location = AppDomain.CurrentDomain.BaseDirectory;
            string folder = Directory.CreateDirectory(Path.Combine(location, nameof(FileWatcherAddedTest))).FullName;

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

            Assert.IsTrue(_fixture.ObservedFiles.Any());
            Assert.AreEqual(count, _fixture.ObservedFiles.Where(q => q.EventName == "Created").Count());
        }

        /// <summary>
        /// Files the watcher deleted test.
        /// </summary>
        public void FileWatcherDeletedTest()
        {
            int count = 1000;

            string location = AppDomain.CurrentDomain.BaseDirectory;
            string folder = Directory.CreateDirectory(Path.Combine(location, nameof(FileWatcherDeletedTest))).FullName;

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

            Assert.IsTrue(_fixture.ObservedFiles.Any());
            Assert.AreEqual(count, _fixture.ObservedFiles.Where(q => q.EventName == "Deleted").Count());
        }
    }
}
