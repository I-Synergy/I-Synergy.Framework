using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ISynergy.Framework.Documents.Extensions;
using System;

namespace Sample.Syncfusion
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                .Build();

                ServiceProvider serviceProvider = new ServiceCollection()
                    .AddLogging()
                    .AddOptions()
                    .AddDocumentsSyncfusionIntegration(config)
                    .AddScoped<Startup>()
                    .BuildServiceProvider();

                Startup application = serviceProvider.GetRequiredService<Startup>();
                await application.RunAsync();
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