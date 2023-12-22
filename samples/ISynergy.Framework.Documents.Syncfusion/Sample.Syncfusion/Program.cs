using ISynergy.Framework.Documents.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sample.Syncfusion;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            IConfigurationRoot config = new ConfigurationBuilder()
            .Build();

            IServiceCollection services = new ServiceCollection()
                .AddLogging()
                .AddOptions();

            services.AddDocumentsSyncfusionIntegration(config);
            services.TryAddScoped<Startup>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();
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