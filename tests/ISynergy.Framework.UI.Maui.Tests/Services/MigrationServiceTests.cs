using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.UI.Services.Tests;

[TestClass]
public class MigrationServiceTests
{
    internal static Dictionary<string, int> _migrations;

    public MigrationServiceTests()
    {
        _migrations = new Dictionary<string, int>();
    }

    [TestMethod]
    public async Task ApplyMigrationAsync_ShouldInvokeUpMethod()
    {
        // Arrange
        var migrationServiceMock = new Mock<IMigrationService>();
        migrationServiceMock.Setup(p => p.ApplyMigrationAsync<SampleMigration_01>()).Callback(() => new SampleMigration_01().Up());
        migrationServiceMock.Setup(p => p.RevertMigrationAsync<SampleMigration_01>()).Callback(() => new SampleMigration_01().Down());
        migrationServiceMock.Setup(p => p.ApplyMigrationAsync<SampleMigration_02>()).Callback(() => new SampleMigration_02().Up());
        migrationServiceMock.Setup(p => p.RevertMigrationAsync<SampleMigration_02>()).Callback(() => new SampleMigration_02().Down());

        // Act
        await migrationServiceMock.Object.ApplyMigrationAsync<SampleMigration_01>();

        // Assert
        migrationServiceMock.Verify(p => p.ApplyMigrationAsync<SampleMigration_01>(), Times.Once);

        Assert.IsTrue(_migrations.ContainsKey(nameof(SampleMigration_01.MigrationVersion)));
        Assert.AreEqual(1, _migrations.GetValueOrDefault(nameof(SampleMigration_01.MigrationVersion), 0));

        // Act
        await migrationServiceMock.Object.ApplyMigrationAsync<SampleMigration_02>();

        // Assert
        migrationServiceMock.Verify(p => p.ApplyMigrationAsync<SampleMigration_02>(), Times.Once);

        Assert.IsTrue(_migrations.ContainsKey(nameof(SampleMigration_02.MigrationVersion)));
        Assert.AreEqual(2, _migrations.GetValueOrDefault(nameof(SampleMigration_02.MigrationVersion), 0));
    }

    [TestMethod]
    public async Task RevertMigrationAsync_ShouldInvokeDownMethod()
    {
        // Arrange
        var migrationServiceMock = new Mock<IMigrationService>();
        migrationServiceMock.Setup(p => p.ApplyMigrationAsync<SampleMigration_01>()).Callback(() => new SampleMigration_01().Up());
        migrationServiceMock.Setup(p => p.RevertMigrationAsync<SampleMigration_01>()).Callback(() => new SampleMigration_01().Down());
        migrationServiceMock.Setup(p => p.ApplyMigrationAsync<SampleMigration_02>()).Callback(() => new SampleMigration_02().Up());
        migrationServiceMock.Setup(p => p.RevertMigrationAsync<SampleMigration_02>()).Callback(() => new SampleMigration_02().Down());

        // Act
        await migrationServiceMock.Object.ApplyMigrationAsync<SampleMigration_01>();

        // Assert
        migrationServiceMock.Verify(p => p.ApplyMigrationAsync<SampleMigration_01>(), Times.Once);

        Assert.IsTrue(_migrations.ContainsKey(nameof(SampleMigration_01.MigrationVersion)));
        Assert.AreEqual(1, _migrations.GetValueOrDefault(nameof(SampleMigration_01.MigrationVersion), 0));

        // Act
        await migrationServiceMock.Object.ApplyMigrationAsync<SampleMigration_02>();

        // Assert
        migrationServiceMock.Verify(p => p.ApplyMigrationAsync<SampleMigration_02>(), Times.Once);

        Assert.IsTrue(_migrations.ContainsKey(nameof(SampleMigration_02.MigrationVersion)));
        Assert.AreEqual(2, _migrations.GetValueOrDefault(nameof(SampleMigration_02.MigrationVersion), 0));

        // Act
        await migrationServiceMock.Object.RevertMigrationAsync<SampleMigration_02>();

        // Assert
        Assert.IsTrue(_migrations.ContainsKey(nameof(SampleMigration_02.MigrationVersion)));
        Assert.AreEqual(1, _migrations.GetValueOrDefault(nameof(SampleMigration_02.MigrationVersion), 0));

        // Act
        await migrationServiceMock.Object.RevertMigrationAsync<SampleMigration_01>();

        // Assert
        migrationServiceMock.Verify(p => p.RevertMigrationAsync<SampleMigration_01>(), Times.Once);

        Assert.IsFalse(_migrations.ContainsKey(nameof(SampleMigration_01.MigrationVersion)));
    }

    public class SampleMigration_01 : IMigration
    {
        public int MigrationVersion => 1;

        public void Up()
        {
            // Add your migration logic here
            _migrations.Add(nameof(MigrationVersion), MigrationVersion);
        }

        public void Down()
        {
            // Add your migration rollback logic here
            _migrations.Remove(nameof(MigrationVersion));
        }
    }

    public class SampleMigration_02 : IMigration
    {
        public int MigrationVersion => 2;

        public void Up()
        {
            // Add your migration logic here
            _migrations[nameof(MigrationVersion)] = MigrationVersion;
        }

        public void Down()
        {
            // Add your migration rollback logic here
            _migrations[nameof(MigrationVersion)] = 1;
        }
    }
}