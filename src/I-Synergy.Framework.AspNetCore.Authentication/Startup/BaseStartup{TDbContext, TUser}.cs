using ISynergy.Framework.AspNetCore.Authentication.Accessors;
using ISynergy.Framework.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.AspNetCore.Authentication.Services;
using ISynergy.Framework.Core.Constants;
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
    public abstract class BaseStartup<TDbContext, TUser> : BaseStartup<TDbContext>, IAsyncInitialization
        where TDbContext : DbContext
        where TUser : IdentityUser
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

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ClaimsAccessor>();
            
            AddAuthorization(services);
            AddMultiTenancy(services);
            AddMultiTenantActionFilter(services);
        }

        /// <summary>
        /// AddMultiTenantActionFilter
        /// </summary>
        /// <param name="services">The services.</param>
        /// <example>
        ///   <code>
        /// services.AddScoped(typeof(TenantActionFilter));
        /// </code>
        /// </example>
        protected virtual void AddMultiTenantActionFilter(IServiceCollection services)
        {
        }

        /// <summary>
        /// Adds the multi tenancy.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void AddMultiTenancy(IServiceCollection services)
        {
            services.AddSingleton<ITenantService, TenantService>();
        }

        /// <summary>
        /// Adds the authorization.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicies.Administrator,
                    policy =>
                    {
                        policy.RequireRole(RoleNames.Administrator);
                        policy.RequireClaim(ClaimTypes.PermissionType, AutorizationRoles.admin_view, AutorizationRoles.admin_create, AutorizationRoles.admin_delete, AutorizationRoles.admin_update);
                    });
                options.AddPolicy(AuthorizationPolicies.User,
                    policy =>
                    {
                        policy.RequireRole(RoleNames.User);
                        policy.RequireClaim(ClaimTypes.PermissionType, AutorizationRoles.user_view);
                    });
            });
        }
    }
}
