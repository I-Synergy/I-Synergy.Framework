using ISynergy.Framework.AspNetCore.Globalization.Constraints;
using ISynergy.Framework.AspNetCore.Globalization.Enumerations;
using ISynergy.Framework.AspNetCore.Globalization.Extensions;
using ISynergy.Framework.AspNetCore.Globalization.Options;
using ISynergy.Framework.AspNetCore.Globalization.Providers;
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.AspNetCore.Globalization.Tests.Extensions
{
    [TestClass]
    public class ApplicationBuilderExtensionsTests
    {
        [TestMethod]
        public void AddGlobalization_RegistersRequiredServices()
        {
            // Arrange
            var services = new ServiceCollection();

            var configValues = new Dictionary<string, string>
            {
                {"GlobalizationOptions:DefaultCulture", "en-US"},
                {"GlobalizationOptions:SupportedCultures:0", "en-US"},
                {"GlobalizationOptions:SupportedCultures:1", "nl-NL"},
                {"GlobalizationOptions:SupportedCultures:2", "fr-FR"},
                {"GlobalizationOptions:ProviderType", "Route"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            // Create a mock IConfigurationManager
            var mockConfigManager = new Mock<IConfigurationManager>();

            // Setup the GetSection method to return sections from our test configuration
            mockConfigManager.Setup(m => m.GetSection(It.IsAny<string>()))
                .Returns<string>(sectionName => configuration.GetSection(sectionName));

            // Create a mock IHostApplicationBuilder
            var mockBuilder = new Mock<IHostApplicationBuilder>();
            mockBuilder.Setup(m => m.Services).Returns(services);
            mockBuilder.Setup(m => m.Configuration).Returns(mockConfigManager.Object);

            // Act
            var result = mockBuilder.Object.AddGlobalization();

            // Assert
            Assert.AreEqual(mockBuilder.Object, result); // Verify method returns the builder

            var serviceProvider = services.BuildServiceProvider();

            // Verify that required services are registered
            var routeProvider = serviceProvider.GetService<RouteDataRequestCultureProvider>();
            Assert.IsNotNull(routeProvider);

            var cultureConstraint = serviceProvider.GetService<CultureRouteConstraint>();
            Assert.IsNotNull(cultureConstraint);

            var languageService = serviceProvider.GetService<ILanguageService>();
            Assert.IsNotNull(languageService);

            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            Assert.IsNotNull(httpContextAccessor);

            // Verify options are configured correctly
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<GlobalizationOptions>>().Value;
            Assert.IsNotNull(options);
            Assert.AreEqual("en-US", options.DefaultCulture);
            Assert.AreEqual(3, options.SupportedCultures.Length);
            Assert.AreEqual(RequestCultureProviderTypes.Route, options.ProviderType);
        }
    }
}
