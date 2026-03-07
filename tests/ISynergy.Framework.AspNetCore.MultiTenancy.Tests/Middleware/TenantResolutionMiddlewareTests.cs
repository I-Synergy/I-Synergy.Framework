using ISynergy.Framework.AspNetCore.MultiTenancy;
using ISynergy.Framework.AspNetCore.MultiTenancy.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Tests.Middleware;

[TestClass]
public class TenantResolutionMiddlewareTests
{
    [TestInitialize]
    public void ResetContext() => TenantContext.Set(Guid.Empty, string.Empty);

    // ------------------------------------------------------------------ //
    // Helpers
    // ------------------------------------------------------------------ //

    private static TenantResolutionMiddleware CreateMiddleware(RequestDelegate? next = null)
        => new(next ?? (_ => Task.CompletedTask));

    private static HttpContext CreateAuthenticatedContext(Guid? tenantId, string? userName = null)
    {
        var claims = new List<Claim>();

        if (tenantId.HasValue)
            claims.Add(new Claim(Claims.KeyId, tenantId.Value.ToString()));

        if (userName is not null)
            claims.Add(new Claim(Claims.Username, userName));

        var identity = new ClaimsIdentity(claims, authenticationType: "Test");
        var principal = new ClaimsPrincipal(identity);

        var context = new DefaultHttpContext();
        context.User = principal;
        return context;
    }

    private static HttpContext CreateUnauthenticatedContext()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity()); // no authentication type → not authenticated
        return context;
    }

    // ------------------------------------------------------------------ //
    // Authenticated requests with valid claims
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task InvokeAsync_AuthenticatedUserWithValidTenantClaim_SetsTenantContext()
    {
        var tenantId = Guid.NewGuid();
        var capturedId = Guid.Empty;
        var middleware = CreateMiddleware(_ => { capturedId = TenantContext.TenantId; return Task.CompletedTask; });
        var context = CreateAuthenticatedContext(tenantId, "alice");

        await middleware.InvokeAsync(context);

        Assert.AreEqual(tenantId, capturedId);
    }

    [TestMethod]
    public async Task InvokeAsync_AuthenticatedUserWithUserNameClaim_SetsUserNameInContext()
    {
        var tenantId = Guid.NewGuid();
        const string expectedUser = "alice";
        var capturedUser = string.Empty;
        var middleware = CreateMiddleware(_ => { capturedUser = TenantContext.UserName; return Task.CompletedTask; });
        var context = CreateAuthenticatedContext(tenantId, expectedUser);

        await middleware.InvokeAsync(context);

        Assert.AreEqual(expectedUser, capturedUser);
    }

    [TestMethod]
    public async Task InvokeAsync_AuthenticatedUserWithoutUserNameClaim_SetsEmptyUserName()
    {
        var tenantId = Guid.NewGuid();
        var capturedUser = "not-empty";
        var middleware = CreateMiddleware(_ => { capturedUser = TenantContext.UserName; return Task.CompletedTask; });
        var context = CreateAuthenticatedContext(tenantId, userName: null);

        await middleware.InvokeAsync(context);

        Assert.AreEqual(string.Empty, capturedUser);
    }

    // ------------------------------------------------------------------ //
    // Authenticated requests with invalid / missing tenant claim
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task InvokeAsync_AuthenticatedUserWithoutTenantClaim_DoesNotSetTenantContext()
    {
        var capturedId = Guid.NewGuid(); // start non-empty to detect if it was written
        var middleware = CreateMiddleware(_ => { capturedId = TenantContext.TenantId; return Task.CompletedTask; });
        var context = CreateAuthenticatedContext(tenantId: null, userName: "bob");

        await middleware.InvokeAsync(context);

        Assert.AreEqual(Guid.Empty, capturedId);
    }

    [TestMethod]
    public async Task InvokeAsync_AuthenticatedUserWithInvalidTenantClaimGuid_DoesNotSetTenantContext()
    {
        var capturedId = Guid.NewGuid();
        var claims = new[] { new Claim(Claims.KeyId, "not-a-guid"), new Claim(Claims.Username, "carol") };
        var identity = new ClaimsIdentity(claims, authenticationType: "Test");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        var middleware = CreateMiddleware(_ => { capturedId = TenantContext.TenantId; return Task.CompletedTask; });

        await middleware.InvokeAsync(context);

        Assert.AreEqual(Guid.Empty, capturedId);
    }

    // ------------------------------------------------------------------ //
    // Unauthenticated requests
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task InvokeAsync_UnauthenticatedUser_DoesNotSetTenantContext()
    {
        var capturedId = Guid.NewGuid();
        var capturedUser = "non-empty";
        var middleware = CreateMiddleware(_ =>
        {
            capturedId = TenantContext.TenantId;
            capturedUser = TenantContext.UserName;
            return Task.CompletedTask;
        });
        var context = CreateUnauthenticatedContext();

        await middleware.InvokeAsync(context);

        Assert.AreEqual(Guid.Empty, capturedId);
        Assert.AreEqual(string.Empty, capturedUser);
    }

    // ------------------------------------------------------------------ //
    // Next delegate is always called
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task InvokeAsync_AuthenticatedRequest_CallsNextMiddleware()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });
        var context = CreateAuthenticatedContext(Guid.NewGuid(), "dave");

        await middleware.InvokeAsync(context);

        Assert.IsTrue(nextCalled);
    }

    [TestMethod]
    public async Task InvokeAsync_UnauthenticatedRequest_CallsNextMiddleware()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });
        var context = CreateUnauthenticatedContext();

        await middleware.InvokeAsync(context);

        Assert.IsTrue(nextCalled);
    }

    [TestMethod]
    public async Task InvokeAsync_MissingTenantClaim_StillCallsNextMiddleware()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });
        var context = CreateAuthenticatedContext(tenantId: null);

        await middleware.InvokeAsync(context);

        Assert.IsTrue(nextCalled);
    }

    // ------------------------------------------------------------------ //
    // TenantContext is available inside the next delegate
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task InvokeAsync_TenantContextAvailableInsideNextDelegate()
    {
        var tenantId = Guid.NewGuid();
        Guid capturedId = Guid.Empty;

        var middleware = CreateMiddleware(_ =>
        {
            capturedId = TenantContext.TenantId;
            return Task.CompletedTask;
        });

        var context = CreateAuthenticatedContext(tenantId, "eve");

        await middleware.InvokeAsync(context);

        Assert.AreEqual(tenantId, capturedId);
    }

    // ------------------------------------------------------------------ //
    // Integration: multiple sequential requests use distinct tenant values
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task InvokeAsync_SequentialRequests_EachRequestSetsItsOwnTenantId()
    {
        // Capture the TenantId visible inside the next delegate for each request.
        var tenant1 = Guid.NewGuid();
        var tenant2 = Guid.NewGuid();
        var captured1 = Guid.Empty;
        var captured2 = Guid.Empty;

        var middleware1 = CreateMiddleware(_ => { captured1 = TenantContext.TenantId; return Task.CompletedTask; });
        var middleware2 = CreateMiddleware(_ => { captured2 = TenantContext.TenantId; return Task.CompletedTask; });

        await middleware1.InvokeAsync(CreateAuthenticatedContext(tenant1, "user1"));
        await middleware2.InvokeAsync(CreateAuthenticatedContext(tenant2, "user2"));

        Assert.AreEqual(tenant1, captured1);
        Assert.AreEqual(tenant2, captured2);
        Assert.AreNotEqual(captured1, captured2);
    }
}
