using System;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Storage.Abstractions;
using ISynergy.Framework.Storage.Azure.Sample;
using ISynergy.Framework.Storage.Azure.Sample.Options;
using ISynergy.Framework.Storage.Azure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Storage.Azure.Sample
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var options = new AzureBlobOptions()
                {
                    ConnectionString = config["AzureBlobOptions:ConnectionString"]
                };

                var services = new ServiceCollection()
                    .AddLogging()
                    .AddOptions()
                    .AddSingleton<IStorageService>(e => new StorageService<AzureBlobOptions>(options, Guid.Parse("ECEB4346-97AD-4919-9248-3EA1012FCA47").ToString()))
                    .AddSingleton<Startup>()
                    .BuildServiceProvider();

                var application = services.GetRequiredService<Startup>();
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
