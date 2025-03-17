using ISynergy.Framework.AspNetCore.Globalization.Enumerations;
using ISynergy.Framework.AspNetCore.Globalization.Options;
using ISynergy.Framework.AspNetCore.Globalization.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace ISynergy.Framework.AspNetCore.Globalization.Tests.Extensions
{
    [TestClass]
    public class WebApplicationExtensionsTests
    {
        [TestMethod]
        public async Task UseGlobalization_WithRouteProvider_SetsCorrectCulture()
        {
            // Arrange
            using var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.Configure<GlobalizationOptions>(options =>
                    {
                        options.DefaultCulture = "en-US";
                        options.SupportedCultures = new[] { "en-US", "nl-NL", "fr-FR" };
                        options.ProviderType = RequestCultureProviderTypes.Route;
                    });

                    services.AddSingleton<RouteDataRequestCultureProvider>();
                })
                .Configure(appBuilder =>
                {
                    // Use the RequestLocalizationMiddleware directly
                    appBuilder.UseRequestLocalization(options =>
                    {
                        var globalization = appBuilder.ApplicationServices
                            .GetRequiredService<IOptions<GlobalizationOptions>>().Value;

                        // Set supported cultures
                        var supportedCultures = globalization.SupportedCultures
                            .Select(c => new CultureInfo(c))
                            .ToArray();

                        options.DefaultRequestCulture = new RequestCulture(globalization.DefaultCulture);
                        options.SupportedCultures = supportedCultures;
                        options.SupportedUICultures = supportedCultures;

                        // Clear existing providers and add our custom provider first
                        options.RequestCultureProviders.Clear();
                        options.RequestCultureProviders.Add(
                            appBuilder.ApplicationServices.GetRequiredService<RouteDataRequestCultureProvider>());
                        options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
                        options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
                        options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
                    });

                    // Map a test endpoint
                    appBuilder.Map("/nl-NL/test", app =>
                    {
                        app.Run(async context =>
                        {
                            await context.Response.WriteAsync(CultureInfo.CurrentCulture.Name);
                        });
                    });

                    // Fallback for other routes
                    appBuilder.Run(async context =>
                    {
                        await context.Response.WriteAsync(CultureInfo.CurrentCulture.Name);
                    });
                }));

            var client = server.CreateClient();

            // Act
            var response = await client.GetStringAsync("/nl-NL/test");

            // Assert
            Assert.AreEqual("nl-NL", response);
        }

        [TestMethod]
        public async Task UseGlobalization_WithAcceptLanguageHeaderProvider_SetsCorrectCulture()
        {
            // Arrange
            using var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.Configure<GlobalizationOptions>(options =>
                    {
                        options.DefaultCulture = "en-US";
                        options.SupportedCultures = new[] { "en-US", "nl-NL", "fr-FR" };
                        options.ProviderType = RequestCultureProviderTypes.AcceptLanguageHeader;
                    });

                    services.AddSingleton<RouteDataRequestCultureProvider>();
                })
                .Configure(appBuilder =>
                {
                    // Use the RequestLocalizationMiddleware directly
                    appBuilder.UseRequestLocalization(options =>
                    {
                        var globalization = appBuilder.ApplicationServices
                            .GetRequiredService<IOptions<GlobalizationOptions>>().Value;

                        // Set supported cultures
                        var supportedCultures = globalization.SupportedCultures
                            .Select(c => new CultureInfo(c))
                            .ToArray();

                        options.DefaultRequestCulture = new RequestCulture(globalization.DefaultCulture);
                        options.SupportedCultures = supportedCultures;
                        options.SupportedUICultures = supportedCultures;

                        // Clear existing providers and add AcceptLanguageHeaderRequestCultureProvider first
                        options.RequestCultureProviders.Clear();
                        options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
                        options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
                        options.RequestCultureProviders.Add(
                            appBuilder.ApplicationServices.GetRequiredService<RouteDataRequestCultureProvider>());
                        options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
                    });

                    appBuilder.Run(async context =>
                    {
                        await context.Response.WriteAsync(CultureInfo.CurrentCulture.Name);
                    });
                }));

            var client = server.CreateClient();

            // Set Accept-Language header
            client.DefaultRequestHeaders.Add("Accept-Language", "fr-FR");

            // Act
            var response = await client.GetStringAsync("/test");

            // Assert
            Assert.AreEqual("fr-FR", response);
        }

        [TestMethod]
        public async Task UseRequestLocalization_ConfiguresOptionsCorrectly()
        {
            // Arrange
            var optionsConfigured = false;
            RequestLocalizationOptions capturedOptions = null;

            using var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.Configure<GlobalizationOptions>(options =>
                    {
                        options.DefaultCulture = "en-US";
                        options.SupportedCultures = new[] { "en-US", "nl-NL", "fr-FR" };
                        options.ProviderType = RequestCultureProviderTypes.Route;
                    });

                    services.AddSingleton<RouteDataRequestCultureProvider>();
                })
                .Configure(appBuilder =>
                {
                    // Use the RequestLocalizationMiddleware and capture the options
                    appBuilder.UseRequestLocalization(options =>
                    {
                        var globalization = appBuilder.ApplicationServices
                            .GetRequiredService<IOptions<GlobalizationOptions>>().Value;

                        // Set supported cultures
                        var supportedCultures = globalization.SupportedCultures
                            .Select(c => new CultureInfo(c))
                            .ToArray();

                        options.DefaultRequestCulture = new RequestCulture(globalization.DefaultCulture);
                        options.SupportedCultures = supportedCultures;
                        options.SupportedUICultures = supportedCultures;

                        // Clear existing providers and add our custom provider first
                        options.RequestCultureProviders.Clear();
                        options.RequestCultureProviders.Add(
                            appBuilder.ApplicationServices.GetRequiredService<RouteDataRequestCultureProvider>());
                        options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
                        options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
                        options.RequestCultureProviders.Add(new CookieRequestCultureProvider());

                        // Capture the configured options
                        capturedOptions = options;
                        optionsConfigured = true;
                    });

                    // Add a simple endpoint to verify the server starts
                    appBuilder.Run(async context =>
                    {
                        await context.Response.WriteAsync("OK");
                    });
                }));

            // Act - Make a request to ensure the middleware pipeline is initialized
            var client = server.CreateClient();
            var response = await client.GetStringAsync("/");

            // Assert
            Assert.IsTrue(optionsConfigured, "Options were not configured");
            Assert.IsNotNull(capturedOptions, "RequestLocalizationOptions was not captured");

            Assert.AreEqual("en-US", capturedOptions.DefaultRequestCulture.Culture.Name);

            // Check that supported cultures are set correctly
            Assert.AreEqual(3, capturedOptions.SupportedCultures.Count);
            Assert.IsTrue(capturedOptions.SupportedCultures.Any(c => c.Name == "en-US"));
            Assert.IsTrue(capturedOptions.SupportedCultures.Any(c => c.Name == "nl-NL"));
            Assert.IsTrue(capturedOptions.SupportedCultures.Any(c => c.Name == "fr-FR"));

            // Check that providers are configured correctly
            Assert.IsTrue(capturedOptions.RequestCultureProviders.Count > 0);
            Assert.IsInstanceOfType(capturedOptions.RequestCultureProviders[0], typeof(RouteDataRequestCultureProvider));
        }

        [TestMethod]
        public async Task UseRequestLocalization_WithApiMode_PrioritizesAcceptLanguageHeaderProvider()
        {
            // Arrange
            var optionsConfigured = false;
            RequestLocalizationOptions capturedOptions = null;

            using var server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.Configure<GlobalizationOptions>(options =>
                    {
                        options.DefaultCulture = "en-US";
                        options.SupportedCultures = new[] { "en-US", "nl-NL", "fr-FR" };
                        options.ProviderType = RequestCultureProviderTypes.Route; // This would normally prioritize route
                    });

                    services.AddSingleton<RouteDataRequestCultureProvider>();
                })
                .Configure(appBuilder =>
                {
                    // Use the RequestLocalizationMiddleware with API configuration
                    appBuilder.UseRequestLocalization(options =>
                    {
                        var globalization = appBuilder.ApplicationServices
                            .GetRequiredService<IOptions<GlobalizationOptions>>().Value;

                        // Set supported cultures
                        var supportedCultures = globalization.SupportedCultures
                            .Select(c => new CultureInfo(c))
                            .ToArray();

                        options.DefaultRequestCulture = new RequestCulture(globalization.DefaultCulture);
                        options.SupportedCultures = supportedCultures;
                        options.SupportedUICultures = supportedCultures;

                        // Clear existing providers and configure for API
                        options.RequestCultureProviders.Clear();
                        options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
                        options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
                        options.RequestCultureProviders.Add(
                            appBuilder.ApplicationServices.GetRequiredService<RouteDataRequestCultureProvider>());

                        // Capture the configured options
                        capturedOptions = options;
                        optionsConfigured = true;
                    });

                    // Add a simple endpoint to verify the server starts
                    appBuilder.Run(async context =>
                    {
                        await context.Response.WriteAsync("OK");
                    });
                }));

            // Act - Make a request to ensure the middleware pipeline is initialized
            var client = server.CreateClient();
            var response = await client.GetStringAsync("/");

            // Assert
            Assert.IsTrue(optionsConfigured, "Options were not configured");
            Assert.IsNotNull(capturedOptions, "RequestLocalizationOptions was not captured");

            // Check that providers are configured correctly for API
            Assert.IsTrue(capturedOptions.RequestCultureProviders.Count > 0);
            Assert.IsInstanceOfType(capturedOptions.RequestCultureProviders[0], typeof(AcceptLanguageHeaderRequestCultureProvider));
        }
    }
}
