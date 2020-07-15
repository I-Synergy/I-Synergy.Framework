using ISynergy.Framework.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.AspNetCore.Startup
{
    /// <summary>
    /// Class BaseStartup.
    /// Implements the <see cref="BaseStartup" />
    /// Implements the <see cref="IAsyncInitialization" />
    /// </summary>
    /// <typeparam name="TDbContext">The type of the t database context.</typeparam>
    /// <seealso cref="BaseStartup" />
    /// <seealso cref="IAsyncInitialization" />
    public abstract class BaseStartup<TDbContext> : BaseStartup, IAsyncInitialization
        where TDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStartup{TDbContext}"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        protected BaseStartup(IWebHostEnvironment environment, IConfiguration configuration)
            : base(environment, configuration)
        {
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            AddDbServices(services);
        }

        /// <summary>
        /// Adds the database services.
        /// </summary>
        /// <param name="services">The services.</param>
        protected abstract void AddDbServices(IServiceCollection services);
    }
}
