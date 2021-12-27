namespace Sample.Synchronization.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
          .ConfigureAppConfiguration(config => {
              config.AddJsonFile("appsettings.json", true, true);
          })
          .ConfigureWebHostDefaults(webBuilder => {
              webBuilder.UseStartup<Startup>();
          });
    }
}
