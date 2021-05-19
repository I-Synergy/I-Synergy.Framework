﻿using System;
using System.IO;
using System.Linq;
using ISynergy.Framework.Core.IO.Tests.Fixtures;
using Xunit;

namespace ISynergy.Framework.Core.IO.Tests
{
    /// <summary>
    /// Class FileWatcherExTests.
    /// Implements the <see cref="IClassFixture{FileWatcherExFixture}" />
    /// </summary>
    /// <seealso cref="IClassFixture{FileWatcherExFixture}" />
    public class FileWatcherExTests : IClassFixture<FileWatcherExFixture>
    {
        /// <summary>
        /// The fixture
        /// </summary>
        private readonly FileWatcherExFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcherExTests"/> class.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        public FileWatcherExTests(FileWatcherExFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Files the watcher ex added test.
        /// </summary>
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

        /// <summary>
        /// Files the watcher ex deleted test.
        /// </summary>
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