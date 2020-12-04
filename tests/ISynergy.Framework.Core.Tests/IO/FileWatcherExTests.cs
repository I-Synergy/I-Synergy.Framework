using System;
using System.IO;
using System.Linq;
using ISynergy.Framework.Core.Tests.Fixtures;
using Xunit;

namespace ISynergy.Framework.Core.IO.Tests
{
    public class FileWatcherExTests : IClassFixture<FileWatcherExFixture>
    {
        private readonly FileWatcherExFixture _fixture;

        public FileWatcherExTests(FileWatcherExFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void FileWatcherExAddedTest()
        {
            var count = 1000;

            var location = AppDomain.CurrentDomain.BaseDirectory;
            var folder = Directory.CreateDirectory(Path.Combine(location, nameof(FileWatcherExAddedTest))).FullName;

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

        [Fact]
        public void FileWatcherExDeletedTest()
        {
            var count = 1000;

            var location = AppDomain.CurrentDomain.BaseDirectory;
            var folder = Directory.CreateDirectory(Path.Combine(location, nameof(FileWatcherExDeletedTest))).FullName;

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
