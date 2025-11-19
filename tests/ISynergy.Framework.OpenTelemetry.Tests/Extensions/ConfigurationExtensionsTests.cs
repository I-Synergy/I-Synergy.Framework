using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.OpenTelemetry.Abstractions.Options;
using ISynergy.Framework.OpenTelemetry.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.OpenTelemetry.Extensions.Tests
{
    [TestClass]
    public class ConfigurationExtensionsTests
    {
        private class TestTelemetryOptions : ITelemetryOptions
        {
            public string? Endpoint { get; set; }
            public string? Protocol { get; set; }
            public TimeSpan Timeout { get; set; }
            public Uri? ServiceUri { get; set; }
            public TestEnum EnumProperty { get; set; }
            public int? NullableInt { get; set; }
            public string? Environment { get; set; }
            public Dictionary<string, string> Attributes { get; } = new Dictionary<string, string>();
            public List<string> ActivitySources { get; } = new List<string>();
            public List<string> MeterNames { get; } = new List<string>();
        }

        public enum TestEnum
        {
            Value1,
            Value2,
            Value3
        }

        [TestMethod]
        public void ConfigureOptions_WithValidConfiguration_SetsProperties()
        {
            // Arrange
            var configValues = new Dictionary<string, string>
            {
                { "Telemetry:Endpoint", "http://localhost:4317" },
                { "Telemetry:Protocol", "grpc" },
                { "Telemetry:Timeout", "00:00:05" },
                { "Telemetry:ServiceUri", "http://localhost:8200" },
                { "Telemetry:EnumProperty", "Value2" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var options = new TestTelemetryOptions();
            configuration.GetSection(TelemetryConstants.TelemetrySection).BindWithReload(options);

            // Assert
            Assert.AreEqual("http://localhost:4317", options.Endpoint);
            Assert.AreEqual("grpc", options.Protocol);
            Assert.AreEqual(TimeSpan.FromSeconds(5), options.Timeout);
            Assert.AreEqual(new Uri("http://localhost:8200"), options.ServiceUri);
            Assert.AreEqual(TestEnum.Value2, options.EnumProperty);
            Assert.AreEqual(0, options.Attributes.Count);
        }

        [TestMethod]
        public void ConfigureOptions_WithTimeSpanAsString_ParsesCorrectly()
        {
            // Arrange
            var configValues = new Dictionary<string, string>
            {
                { "Telemetry:Timeout", "00:00:30" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var options = new TestTelemetryOptions();
            configuration.GetSection(TelemetryConstants.TelemetrySection).BindWithReload(options);

            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(30), options.Timeout);
        }

        [TestMethod]
        public void ConfigureOptions_WithNullableType_SetsValue()
        {
            // Arrange
            var configValues = new Dictionary<string, string>
            {
                { "Telemetry:NullableInt", "42" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var options = new TestTelemetryOptions();

            // Act
            configuration.GetSection(TelemetryConstants.TelemetrySection).BindWithReload(options);

            // Assert
            Assert.AreEqual(42, options.NullableInt);
        }

        [TestMethod]
        public void ConfigureOptions_WithInvalidValue_SkipsProperty()
        {
            // Arrange
            var configValues = new Dictionary<string, string>
            {
                { "Telemetry:EnumProperty", "InvalidValue" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var options = new TestTelemetryOptions();
            options.EnumProperty = TestEnum.Value1; // Set default value

            //Act
            configuration.GetSection(TelemetryConstants.TelemetrySection).BindWithReload(options);

            // Assert - value should remain unchanged
            Assert.AreEqual(TestEnum.Value1, options.EnumProperty);
        }

        [TestMethod]
        public void ConfigureOptions_WithCustomSection_UsesCorrectSection()
        {
            // Arrange
            var configValues = new Dictionary<string, string>
            {
                { "CustomSection:Endpoint", "http://custom.endpoint" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var options = new TestTelemetryOptions();

            // Act
            configuration.GetSection("CustomSection").BindWithReload(options);

            // Assert
            Assert.AreEqual("http://custom.endpoint", options.Endpoint);
        }

        [TestMethod]
        public void ConfigureOptions_WithNonExistentSection_DoesNotModifyOptions()
        {
            // Arrange
            var configValues = new Dictionary<string, string>
            {
                { "SomeOtherSection:Endpoint", "http://other.endpoint" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var options = new TestTelemetryOptions
            {
                Endpoint = "original"
            };

            // Act
            configuration.GetSection("NonExistentSection").BindWithReload(options);

            // Assert
            Assert.AreEqual("original", options.Endpoint);
        }

        [TestMethod]
        public void ConfigureOptions_WithAdditionalOptions_AddsToAttributes()
        {
            // Arrange
            var configValues = new Dictionary<string, string>
            {
                { "Telemetry:Endpoint", "http://localhost:4317" },
                { "Telemetry:Attributes:CustomOption1", "value1" },
                { "Telemetry:Attributes:CustomOption2", "value2" },
                { "Telemetry:environment", "test" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var options = new TestTelemetryOptions();

            // Act
            configuration.GetSection(TelemetryConstants.TelemetrySection).BindWithReload(options);

            // Assert
            Assert.AreEqual("http://localhost:4317", options.Endpoint);
            Assert.AreEqual(2, options.Attributes.Count);
            Assert.AreEqual("value1", options.Attributes["CustomOption1"]);
            Assert.AreEqual("value2", options.Attributes["CustomOption2"]);
            Assert.AreEqual("test", options.Environment);
        }

        [TestMethod]
        public void ConfigureOptions_WithJsonFile_LoadsCorrectly()
        {
            // Arrange
            string json = @"{
                ""Telemetry"": {
                    ""Endpoint"": ""http://json.endpoint"",
                    ""Protocol"": ""http/protobuf"",
                    ""Timeout"": ""00:00:03"",
                    ""CustomAttribute"": ""custom-value""
                }
            }";

            string tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, json);

            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile(tempFile)
                    .Build();

                var options = new TestTelemetryOptions();

                // Act
                configuration.GetSection(TelemetryConstants.TelemetrySection).BindWithReload(options);

                // Assert
                Assert.AreEqual("http://json.endpoint", options.Endpoint);
                Assert.AreEqual("http/protobuf", options.Protocol);
                Assert.AreEqual(TimeSpan.FromMilliseconds(3000), options.Timeout);
            }
            finally
            {
                // Clean up
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [TestMethod]
        public void ConfigureOptions_WithCaseSensitivity_HandlesCorrectly()
        {
            // Arrange
            var configValues = new Dictionary<string, string>
            {
                { "Telemetry:endpoint", "http://lowercase.endpoint" },
                { "Telemetry:PROTOCOL", "uppercase-protocol" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var options = new TestTelemetryOptions();

            // Act
            configuration.GetSection(TelemetryConstants.TelemetrySection).BindWithReload(options);

            // Assert
            Assert.AreEqual("http://lowercase.endpoint", options.Endpoint);
            Assert.AreEqual("uppercase-protocol", options.Protocol);
            Assert.AreEqual(0, options.Attributes.Count); // These should be recognized as properties, not attributes
        }
    }
}
