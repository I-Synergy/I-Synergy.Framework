using ISynergy.Framework.MessageBus.Azure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.MessageBus.Models;
using System;

namespace Sample.MessageBus.Subscriber;

/// <summary>
/// Class Program.
/// </summary>
internal class Program
{
    protected Program()
    {
    }

    static int Main(string[] args)
    {
        try
        {
            IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();

            IServiceCollection services = new ServiceCollection()
                .AddLogging()
                .AddOptions();

            services.AddMessageBusAzureSubscribeIntegration<TestDataModel>(config);
            services.TryAddScoped<Startup>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

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
