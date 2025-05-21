using ISynergy.Framework.Storage.Azure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sample.Storage.Azure;

internal class Program
{
    protected Program()
    {
    }

    static int Main(string[] args)
    {
        try
        {
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .Build();

            IServiceCollection services = new ServiceCollection()
                .AddLogging()
                .AddOptions();

            services.AddAzureStorageIntegration();
            services.TryAddScoped<Startup>();

            var serviceProvider = services.BuildServiceProvider();
            Startup application = serviceProvider.GetRequiredService<Startup>();
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
