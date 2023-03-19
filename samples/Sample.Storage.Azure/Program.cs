using ISynergy.Framework.Storage.Azure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Storage.Azure.Options;
using System;

namespace Sample.Storage.Azure
{
    internal class Program
    {
        protected Program()
        {
        }

        static int Main(string[] args)
        {
            try
            {
                string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                    .Build();

                AzureBlobOptions options = new()
                {
                    ConnectionString = config["AzureBlobOptions:ConnectionString"]
                };

                ServiceProvider services = new ServiceCollection()
                    .AddLogging()
                    .AddOptions()
                    .AddStorageAzureIntegration<AzureBlobOptions>(config, Guid.Parse("ECEB4346-97AD-4919-9248-3EA1012FCA47").ToString())
                    .AddSingleton<Startup>()
                    .BuildServiceProvider();

                Startup application = services.GetRequiredService<Startup>();
                application.RunAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 1;
            }

            return 0;
        }
    }
}
