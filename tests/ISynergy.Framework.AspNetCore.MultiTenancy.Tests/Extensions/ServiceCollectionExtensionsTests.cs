using ISynergy.Framework.AspNetCore.MultiTenancy.Extensions;
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Tests.Extensions;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    // ------------------------------------------------------------------ //
    // AddMultiTenancyIntegration
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void AddMultiTenancyIntegration_RegistersITenantService()
    {
        var services = new ServiceCollection();
        services.AddMultiTenancyIntegration();

        var provider = services.BuildServiceProvider();
        var tenantService = provider.GetService<ITenantService>();

        Assert.IsNotNull(tenantService);
    }

    [TestMethod]
    public void AddMultiTenancyIntegration_ITenantService_IsTransient()
    {
        var services = new ServiceCollection();
        services.AddMultiTenancyIntegration();

        var provider = services.BuildServiceProvider();

        var instance1 = provider.GetRequiredService<ITenantService>();
        var instance2 = provider.GetRequiredService<ITenantService>();

        Assert.AreNotSame(instance1, instance2);
    }

    [TestMethod]
    public void AddMultiTenancyIntegration_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        var result = services.AddMultiTenancyIntegration();

        Assert.AreSame(services, result);
    }

    [TestMethod]
    public void AddMultiTenancyIntegration_CalledTwice_DoesNotDuplicateRegistration()
    {
        var services = new ServiceCollection();
        services.AddMultiTenancyIntegration();
        services.AddMultiTenancyIntegration();

        var registrations = services.Where(d => d.ServiceType == typeof(ITenantService)).ToList();

        // TryAddTransient means only one registration survives.
        Assert.AreEqual(1, registrations.Count);
    }

    [TestMethod]
    public void AddMultiTenancyIntegration_DoesNotRegisterIHttpContextAccessor()
    {
        var services = new ServiceCollection();
        services.AddMultiTenancyIntegration();

        var provider = services.BuildServiceProvider();

        // IHttpContextAccessor is no longer required by the new TenantService.
        // Verify it is NOT auto-registered (consumer must add it separately if needed).
        var httpContextAccessor = provider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        Assert.IsNull(httpContextAccessor);
    }

    // ------------------------------------------------------------------ //
    // UseMultiTenancyMiddleware
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task UseMultiTenancyMiddleware_MiddlewareInPipeline_InvokesCorrectly()
    {
        // Build a real TestServer to confirm the middleware integrates without error.
        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                        services.AddMultiTenancyIntegration();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseMultiTenancyMiddleware();
                        app.UseEndpoints(e => e.MapGet("/ping", () => "pong"));
                    });
            })
            .StartAsync();

        var client = host.GetTestClient();
        var response = await client.GetAsync("/ping");

        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task UseMultiTenancyMiddleware_UnauthenticatedRequest_PipelineContinues()
    {
        var nextInvoked = false;

        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                        services.AddMultiTenancyIntegration();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseMultiTenancyMiddleware();
                        app.Use(async (_, next) =>
                        {
                            nextInvoked = true;
                            await next();
                        });
                        app.UseEndpoints(e => e.MapGet("/", () => string.Empty));
                    });
            })
            .StartAsync();

        var client = host.GetTestClient();
        await client.GetAsync("/");

        Assert.IsTrue(nextInvoked);
    }
}
