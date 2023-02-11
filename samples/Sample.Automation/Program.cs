using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ISynergy.Framework.AspNetCore.Sample
{
    public class Program
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected Program() { }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
