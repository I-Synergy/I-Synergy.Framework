using ISynergy.Framework.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ISynergy.Framework.AspNetCore.Startup;

namespace ISynergy.Framework.AspNetCore.Authentication
{
    /// <summary>
    /// Class BaseStartup.
    /// Implements the <see cref="BaseStartup{TDbContext}" />
    /// Implements the <see cref="IAsyncInitialization" />
    /// </summary>
    /// <typeparam name="TDbContext">The type of the t database context.</typeparam>
    /// <typeparam name="TUser">The type of the t user.</typeparam>
    /// <seealso cref="BaseStartup{TDbContext}" />
    /// <seealso cref="IAsyncInitialization" />
    public abstract class BaseStartup<TDbContext, TUser> : BaseStartup<TDbContext, TUser, string>, IAsyncInitialization
        where TDbContext : DbContext
        where TUser : IdentityUser<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStartup{TDbContext, TUser}"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        protected BaseStartup(IWebHostEnvironment environment, IConfiguration configuration)
            : base(environment, configuration)
        {
        }
    }
}
