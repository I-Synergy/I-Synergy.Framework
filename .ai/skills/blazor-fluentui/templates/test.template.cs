/*
 * TEMPLATE: bUnit Component Test (Lines: ~250)
 * Purpose: Unit tests for Blazor components
 * Placeholders: {{ComponentName}}, {{Namespace}}, {{PropertyName}}
 */

using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using {{Namespace}}.Components;
using {{Namespace}}.Services;

namespace {{Namespace}}.Tests.Components;

[TestClass]
public class {{ComponentName}}Tests
{
    // MSTest owns the name TestContext, so bUnit's context is referenced as Bunit.TestContext
    private Bunit.TestContext _ctx = null!;
    private Mock<I{{ServiceName}}> _mockService = null!;

    [TestInitialize]
    public void Setup()
    {
        _ctx = new Bunit.TestContext();
        _mockService = new Mock<I{{ServiceName}}>();

        // Register services
        _ctx.Services.AddSingleton(_mockService.Object);
        _ctx.Services.AddFluentUIComponents();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _ctx.Dispose();
    }

    [TestMethod]
    public void Component_Renders_WithRequiredParameters()
    {
        // Arrange
        var {{propertyName}} = "Test {{PropertyName}}";

        // Act
        var cut = _ctx.RenderComponent<{{ComponentName}}>(parameters => parameters
            .Add(p => p.{{PropertyName}}, {{propertyName}}));

        // Assert
        cut.Find("h3").TextContent.Should().Be({{propertyName}});
    }

    [TestMethod]
    public void Component_Shows_ChildContent_WhenProvided()
    {
        // Arrange
        var childContent = "Custom child content";

        // Act
        var cut = _ctx.RenderComponent<{{ComponentName}}>(parameters => parameters
            .Add(p => p.{{PropertyName}}, "Title")
            .AddChildContent(childContent));

        // Assert
        cut.Markup.Should().Contain(childContent);
    }

    [TestMethod]
    public async Task PrimaryButton_Click_InvokesCallback()
    {
        // Arrange
        var callbackInvoked = false;
        var cut = _ctx.RenderComponent<{{ComponentName}}>(parameters => parameters
            .Add(p => p.{{PropertyName}}, "Test")
            .Add(p => p.OnPrimaryAction, () => { callbackInvoked = true; }));

        // Act
        var button = cut.Find("button");
        await button.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        // Assert
        callbackInvoked.Should().BeTrue();
    }

    [TestMethod]
    public void Component_Shows_LoadingIndicator_WhenProcessing()
    {
        // Arrange
        var cut = _ctx.RenderComponent<{{ComponentName}}>(parameters => parameters
            .Add(p => p.{{PropertyName}}, "Test"));

        // Act
        cut.Instance.TriggerPrimaryActionAsync();
        cut.WaitForState(() => cut.Find("fluent-progress-ring") != null, timeout: TimeSpan.FromSeconds(2));

        // Assert
        cut.FindAll("fluent-progress-ring").Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task Component_Handles_ServiceErrors_Gracefully()
    {
        // Arrange
        _mockService.Setup(s => s.GetDataAsync())
            .ThrowsAsync(new Exception("Service error"));

        var cut = _ctx.RenderComponent<{{ComponentName}}>(parameters => parameters
            .Add(p => p.{{PropertyName}}, "Test"));

        // Act & Assert
        await cut.Instance.TriggerPrimaryActionAsync();
        cut.Markup.Should().Contain("error");
    }

    [TestMethod]
    public void Component_DisablesButtons_WhileLoading()
    {
        // Arrange
        var cut = _ctx.RenderComponent<{{ComponentName}}>(parameters => parameters
            .Add(p => p.{{PropertyName}}, "Test"));

        // Act
        cut.Instance.TriggerPrimaryActionAsync();

        // Assert
        cut.FindAll("button[disabled]").Should().NotBeEmpty();
    }

    [TestMethod]
    public async Task Component_EmitsStateChangedEvent()
    {
        // Arrange
        ComponentState? emittedState = null;
        var cut = _ctx.RenderComponent<{{ComponentName}}>(parameters => parameters
            .Add(p => p.{{PropertyName}}, "Test")
            .Add(p => p.OnStateChanged, state => { emittedState = state; }));

        // Act
        await cut.Instance.TriggerPrimaryActionAsync();

        // Assert
        emittedState.Should().NotBeNull();
    }

    [TestMethod]
    public void Component_Reset_ClearsState()
    {
        // Arrange
        var cut = _ctx.RenderComponent<{{ComponentName}}>(parameters => parameters
            .Add(p => p.{{PropertyName}}, "Test"));

        // Act
        cut.Instance.Reset();

        // Assert
        cut.Instance.Should().NotBeNull();
        // Verify state is cleared
    }
}

// Integration test example
[TestClass]
public class {{ComponentName}}IntegrationTests
{
    private Bunit.TestContext _ctx = null!;

    [TestInitialize]
    public void Setup()
    {
        _ctx = new Bunit.TestContext();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _ctx.Dispose();
    }

    [TestMethod]
    public async Task FullWorkflow_CreateToSave_Works()
    {
        // Arrange
        var service = new {{ServiceName}}(Mock.Of<ILogger<{{ServiceName}}>>());
        _ctx.Services.AddSingleton<I{{ServiceName}}>(service);

        var cut = _ctx.RenderComponent<{{ComponentName}}>(parameters => parameters
            .Add(p => p.{{PropertyName}}, "Integration Test"));

        // Act
        await cut.Instance.TriggerPrimaryActionAsync();

        // Assert
        service.Items.Should().NotBeEmpty();
    }
}
