using ISynergy.Framework.Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Middleware;

[TestClass]
public class TenantResolutionMiddlewareTests
{
    [TestInitialize]
    public void ResetContext() => TenantContext.Set(Guid.Empty, string.Empty);

    // ------------------------------------------------------------------ //
    // Helpers
    // ------------------------------------------------------------------ //

    private static TenantResolutionMiddleware CreateMiddleware(
        RequestDelegate? next = null,
        TenantResolutionOptions? options = null)
        => new(
            next ?? (_ => Task.CompletedTask),
            Microsoft.Extensions.Options.Options.Create(options ?? new TenantResolutionOptions()),
            NullLogger<TenantResolutionMiddleware>.Instance);

    private static HttpContext CreateAuthenticatedContext(Guid? tenantId, string? userName = null)
    {
        var claims = new List<Claim>();

        if (tenantId.HasValue)
            claims.Add(new Claim(Claims.TenantId, tenantId.Value.ToString()));

        if (userName is not null)
            claims.Add(new Claim(Claims.Username, userName));

        return CreateContext(claims);
    }

    private static HttpContext CreateContext(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "Test");
        return new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
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
    // Configurable claim names and fallback
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task InvokeAsync_ConfiguredClaimNames_ResolveTenantAndUserName()
    {
        var tenantId = Guid.NewGuid();
        var options = new TenantResolutionOptions
        {
            TenantIdClaimType = "tenant_id",
            UsernameClaimType = "preferred_username"
        };
        var capturedId = Guid.Empty;
        var capturedUser = string.Empty;
        var middleware = CreateMiddleware(_ =>
        {
            capturedId = TenantContext.TenantId;
            capturedUser = TenantContext.UserName;
            return Task.CompletedTask;
        }, options);
        var context = CreateContext(new[]
        {
            new Claim("tenant_id", tenantId.ToString()),
            new Claim("preferred_username", "alice")
        });

        await middleware.InvokeAsync(context);

        Assert.AreEqual(tenantId, capturedId);
        Assert.AreEqual("alice", capturedUser);
    }

    [TestMethod]
    public async Task InvokeAsync_PrimaryTenantClaimMissing_UsesFallbackClaim()
    {
        var fallbackTenant = Guid.NewGuid();
        var options = new TenantResolutionOptions
        {
            TenantIdClaimType = "tenant_id",
            TenantIdFallbackClaimType = "sub"
        };
        var capturedId = Guid.Empty;
        var middleware = CreateMiddleware(_ => { capturedId = TenantContext.TenantId; return Task.CompletedTask; }, options);
        var context = CreateContext(new[] { new Claim("sub", fallbackTenant.ToString()) });

        await middleware.InvokeAsync(context);

        Assert.AreEqual(fallbackTenant, capturedId);
    }

    [TestMethod]
    public async Task InvokeAsync_UsernameClaimTypeNull_SetsEmptyUserName()
    {
        var tenantId = Guid.NewGuid();
        var options = new TenantResolutionOptions
        {
            UsernameClaimType = null
        };
        var capturedId = Guid.Empty;
        var capturedUser = "not-empty";
        var middleware = CreateMiddleware(_ =>
        {
            capturedId = TenantContext.TenantId;
            capturedUser = TenantContext.UserName;
            return Task.CompletedTask;
        }, options);
        var context = CreateContext(new[]
        {
            new Claim(Claims.TenantId, tenantId.ToString()),
            new Claim(Claims.Username, "should-be-ignored")
        });

        await middleware.InvokeAsync(context);

        Assert.AreEqual(tenantId, capturedId);
        Assert.AreEqual(string.Empty, capturedUser);
    }

    // ------------------------------------------------------------------ //
    // Authenticated requests with invalid / missing tenant claim
    //
    // RequireTenant defaults to true, so an authenticated principal whose tenant
    // cannot be resolved is rejected (fail loud) rather than silently continuing
    // with an un-tenanted context — the tenant-bypass guard from commit ea702517.
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task InvokeAsync_AuthenticatedUserWithoutTenantClaim_Throws()
    {
        var middleware = CreateMiddleware(_ => Task.CompletedTask);
        var context = CreateAuthenticatedContext(tenantId: null, userName: "bob");

        await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));
    }

    [TestMethod]
    public async Task InvokeAsync_AuthenticatedUserWithInvalidTenantClaimGuid_Throws()
    {
        var claims = new[] { new Claim(Claims.TenantId, "not-a-guid"), new Claim(Claims.Username, "carol") };
        var context = CreateContext(claims);
        var middleware = CreateMiddleware(_ => Task.CompletedTask);

        await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));
    }

    [TestMethod]
    public async Task InvokeAsync_RequireTenantFalse_MissingTenantClaim_DoesNotThrowAndLeavesContextUnset()
    {
        var capturedId = Guid.NewGuid(); // start non-empty to detect if it was written
        var options = new TenantResolutionOptions { RequireTenant = false };
        var middleware = CreateMiddleware(_ => { capturedId = TenantContext.TenantId; return Task.CompletedTask; }, options);
        var context = CreateAuthenticatedContext(tenantId: null, userName: "bob");

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
    // Next delegate is always called for resolvable / unauthenticated requests
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
    public async Task InvokeAsync_MissingTenantClaim_ThrowsBeforeCallingNext()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });
        var context = CreateAuthenticatedContext(tenantId: null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));

        Assert.IsFalse(nextCalled);
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
