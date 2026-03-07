using ISynergy.Framework.AspNetCore.MultiTenancy;
using ISynergy.Framework.AspNetCore.MultiTenancy.Extensions;
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Tests.Services;

[TestClass]
public class TenantServiceTests
{
    private readonly ITenantService _tenantService;

    public TenantServiceTests()
    {
        var services = new ServiceCollection();
        services.AddMultiTenancyIntegration();
        _tenantService = services.BuildServiceProvider().GetRequiredService<ITenantService>();
    }

    [TestInitialize]
    public void ResetContext() => TenantContext.Set(Guid.Empty, string.Empty);

    // ------------------------------------------------------------------ //
    // TenantId property
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void TenantId_ReflectsTenantContextTenantId()
    {
        var expected = Guid.NewGuid();
        TenantContext.Set(expected, "user");

        Assert.AreEqual(expected, _tenantService.TenantId);
    }

    [TestMethod]
    public void TenantId_WhenContextEmpty_ReturnsGuidEmpty()
    {
        Assert.AreEqual(Guid.Empty, _tenantService.TenantId);
    }

    // ------------------------------------------------------------------ //
    // UserName property
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void UserName_ReflectsTenantContextUserName()
    {
        TenantContext.Set(Guid.NewGuid(), "bob");

        Assert.AreEqual("bob", _tenantService.UserName);
    }

    [TestMethod]
    public void UserName_WhenContextEmpty_ReturnsEmptyString()
    {
        Assert.AreEqual(string.Empty, _tenantService.UserName);
    }

    // ------------------------------------------------------------------ //
    // SetTenant(Guid)
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void SetTenant_WithTenantIdOnly_WritesTenantIdToContext()
    {
        var tenantId = Guid.NewGuid();

        _tenantService.SetTenant(tenantId);

        Assert.AreEqual(tenantId, TenantContext.TenantId);
    }

    [TestMethod]
    public void SetTenant_WithTenantIdOnly_LeavesUserNameEmpty()
    {
        _tenantService.SetTenant(Guid.NewGuid());

        Assert.AreEqual(string.Empty, TenantContext.UserName);
    }

    // ------------------------------------------------------------------ //
    // SetTenant(Guid, string)
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void SetTenant_WithTenantIdAndUserName_WritesBothToContext()
    {
        var tenantId = Guid.NewGuid();
        const string userName = "charlie";

        _tenantService.SetTenant(tenantId, userName);

        Assert.AreEqual(tenantId, TenantContext.TenantId);
        Assert.AreEqual(userName, TenantContext.UserName);
    }

    [TestMethod]
    public void SetTenant_CalledTwice_OverwritesPreviousValues()
    {
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();

        _tenantService.SetTenant(first, "user1");
        _tenantService.SetTenant(second, "user2");

        Assert.AreEqual(second, _tenantService.TenantId);
        Assert.AreEqual("user2", _tenantService.UserName);
    }

    // ------------------------------------------------------------------ //
    // Round-trip: SetTenant then read via properties
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void SetTenantAndRead_ValuesRoundTripCorrectly()
    {
        var tenantId = Guid.NewGuid();
        const string userName = "diana";

        _tenantService.SetTenant(tenantId, userName);

        Assert.AreEqual(tenantId, _tenantService.TenantId);
        Assert.AreEqual(userName, _tenantService.UserName);
    }

    // ------------------------------------------------------------------ //
    // Interaction with TenantContext.Use()
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void TenantService_ReadsFromContextSetByUse()
    {
        var tenantId = Guid.NewGuid();

        using (TenantContext.Use(tenantId, "worker"))
        {
            Assert.AreEqual(tenantId, _tenantService.TenantId);
            Assert.AreEqual("worker", _tenantService.UserName);
        }

        Assert.AreEqual(Guid.Empty, _tenantService.TenantId);
    }
}
