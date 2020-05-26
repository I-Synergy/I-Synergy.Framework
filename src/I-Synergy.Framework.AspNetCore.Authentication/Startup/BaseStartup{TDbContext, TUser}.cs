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
    public abstract class BaseStartup<TDbContext, TUser> : BaseStartup<TDbContext>, IAsyncInitialization
        where TDbContext : DbContext
        where TUser : IdentityUser
    {
        protected BaseStartup(IWebHostEnvironment environment, IConfiguration configuration)
            : base(environment, configuration)
        {
        }

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
        /// <param name="services"></param>
        /// <example>
        /// <code>
        /// services.AddScoped(typeof(TenantActionFilter));
        /// </code>
        /// </example>
        protected virtual void AddMultiTenantActionFilter(IServiceCollection services)
        {
        }

        protected virtual void AddMultiTenancy(IServiceCollection services)
        {
            services.AddSingleton<ITenantService, TenantService>();
        }

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
