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
        migrationServiceMock.Setup(p => p.ApplyMigrationAsync<SampleMigration>()).Callback(() => new SampleMigration().Up());
        migrationServiceMock.Setup(p => p.RevertMigrationAsync<SampleMigration>()).Callback(() => new SampleMigration().Down());

        // Act
        await migrationServiceMock.Object.ApplyMigrationAsync<SampleMigration>();

        // Assert
        migrationServiceMock.Verify(p => p.ApplyMigrationAsync<SampleMigration>(), Times.Once);

        Assert.IsTrue(_migrations.ContainsKey(nameof(SampleMigration.MigrationVersion)));
        Assert.AreEqual(1975, _migrations.GetValueOrDefault(nameof(SampleMigration.MigrationVersion), 0));
    }

    [TestMethod]
    public async Task RevertMigrationAsync_ShouldInvokeDownMethod()
    {
        // Arrange
        var migrationServiceMock = new Mock<IMigrationService>();
        migrationServiceMock.Setup(p => p.ApplyMigrationAsync<SampleMigration>()).Callback(() => new SampleMigration().Up());
        migrationServiceMock.Setup(p => p.RevertMigrationAsync<SampleMigration>()).Callback(() => new SampleMigration().Down());


        // Act
        await migrationServiceMock.Object.ApplyMigrationAsync<SampleMigration>();

        // Assert
        Assert.IsTrue(_migrations.ContainsKey(nameof(SampleMigration.MigrationVersion)));
        Assert.AreEqual(1975, _migrations.GetValueOrDefault(nameof(SampleMigration.MigrationVersion), 0));

        // Act
        await migrationServiceMock.Object.RevertMigrationAsync<SampleMigration>();

        // Assert
        migrationServiceMock.Verify(p => p.RevertMigrationAsync<SampleMigration>(), Times.Once);

        Assert.IsFalse(_migrations.ContainsKey(nameof(SampleMigration.MigrationVersion)));
    }

    public class SampleMigration : IMigration
    {
        public string MigrationVersion => "SampleMigration";

        public void Up()
        {
            // Add your migration logic here
            _migrations.Add(nameof(MigrationVersion), 1975);
        }

        public void Down()
        {
            // Add your migration rollback logic here
            _migrations.Remove(nameof(MigrationVersion));
        }
    }
}