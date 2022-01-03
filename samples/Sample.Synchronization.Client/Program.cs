using ISynergy.Framework.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Synchronization.Client.Extensions;
using Sample.Synchronization.Common.Abstractions;
using Sample.Synchronization.Common.Messages;
using Sample.Synchronization.Common.Options;

namespace Sample.Synchronization.Client
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var context = Context.GetInstance();
                var syncSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", $"{nameof(ClientSynchronizationOptions)}.json");
                var fileSyncSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", $"{nameof(FileSynchronizationOptions)}.json");

                if (File.Exists(syncSettingsPath) && File.Exists(fileSyncSettingsPath))
                {
                    var config = new ConfigurationBuilder()
                        .AddJsonFile(syncSettingsPath, false)
                        .AddJsonFile(fileSyncSettingsPath, false)
                        .AddEnvironmentVariables()
                        .Build();

                    if(config.Get<ClientSynchronizationOptions>() is ClientSynchronizationOptions options &&
                        config.Get<FileSynchronizationOptions>() is FileSynchronizationOptions fileSyncOptions &&
                        !string.IsNullOrEmpty(options.Host) &&
                        !string.IsNullOrEmpty(fileSyncOptions.Host)
                        )
                    {
                        var serviceProvider = new ServiceCollection()
                        .AddLogging()
                        .AddOptions()
                        .AddSyncService(config)
                        .BuildServiceProvider();

                        MessageService.Default.Register<IsOnlineMessage>(null, m => {
                            context.IsOffline = !m.Content;
                            Console.WriteLine("Online state received and set in the application context.");
                        });

                        serviceProvider.GetRequiredService<ISynchronizationService>();
                    }
                }

                Console.Read();
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
