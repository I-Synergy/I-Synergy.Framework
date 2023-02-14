using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Logging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Sample.ApplicationInsights
{
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
                var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

                var assembly = Assembly.GetAssembly(typeof(Program));

                var serviceProvider = new ServiceCollection()
                    .AddLogging(builder => builder.AddApplicationInsightsLogging(config))
                    .AddOptions()
                    .AddSingleton<IContext, Context>()
                    .AddSingleton<IInfoService>(s => new InfoService(assembly))
                    .AddScoped<Startup>()
                    .BuildServiceProvider();

                var application = serviceProvider.GetRequiredService<Startup>();
                application.Run();
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