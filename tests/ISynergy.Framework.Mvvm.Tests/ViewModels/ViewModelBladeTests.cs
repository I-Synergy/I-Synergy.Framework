﻿using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelBladeTests
{
    private Mock<IContext> _mockContext;
    private Mock<IBaseCommonServices> _mockCommonServices;
    private Mock<ILogger> _mockLogger;

    [TestInitialize]
    public void Setup()
    {
        _mockContext = new Mock<IContext>();
        _mockCommonServices = new Mock<IBaseCommonServices>();
        _mockLogger = new Mock<ILogger>();
    }

    private class TestBladeViewModel : ViewModelBlade<TestEntity>
    {
        public TestBladeViewModel(IContext context, IBaseCommonServices commonServices, ILogger logger, bool automaticValidation = false)
            : base(context, commonServices, logger, automaticValidation) { }
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [TestMethod]
    public void Constructor_InitializesProperties()
    {
        // Arrange & Act
        var viewModel = new TestBladeViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);

        // Assert
        Assert.IsNotNull(viewModel.SubmitCommand);
        Assert.IsFalse(viewModel.IsUpdate);
        Assert.IsNull(viewModel.Owner);
        Assert.IsFalse(viewModel.IsDisabled);
    }

    [TestMethod]
    public async Task SubmitAsync_WithValidation_InvokesSubmittedEvent()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var eventInvoked = false;
        viewModel.Submitted += (s, e) => eventInvoked = true;

        // Act
        await viewModel.SubmitAsync(entity);

        // Assert
        Assert.IsTrue(eventInvoked);
    }

    [TestMethod]
    public void SetSelectedItem_UpdatesPropertyAndFlag()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
        var entity = new TestEntity { Id = 1, Name = "Test" };

        // Act
        viewModel.SelectedItem = entity;

        // Assert
        Assert.AreEqual(entity, viewModel.SelectedItem);
    }

    [TestMethod]
    public void SetOwner_UpdatesOwnerProperty()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
        var mockOwner = new Mock<IViewModelBladeView>();

        // Act
        viewModel.Owner = mockOwner.Object;

        // Assert
        Assert.AreEqual(mockOwner.Object, viewModel.Owner);
    }

    [TestMethod]
    public async Task SubmitAsync_WithValidation_InvokesSubmittedAndCloses()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var submittedInvoked = false;
        var closedInvoked = false;
        viewModel.Submitted += (s, e) => submittedInvoked = true;
        viewModel.Closed += (s, e) => closedInvoked = true;

        // Act
        await viewModel.SubmitAsync(entity);

        // Assert
        Assert.IsTrue(submittedInvoked);
        Assert.IsTrue(closedInvoked);
    }

    [TestMethod]
    public void Cleanup_ClearsSelectedItemAndCommands()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockContext.Object, _mockCommonServices.Object, _mockLogger.Object);
        viewModel.SelectedItem = new TestEntity();

        // Act
        viewModel.Cleanup();

        // Assert
        Assert.IsNull(viewModel.SelectedItem);
        Assert.IsNull(viewModel.SubmitCommand);
    }
}
