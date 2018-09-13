using AspNet.Security.OpenIdConnect.Primitives;
using ISynergy.Accessors;
using ISynergy.Extensions;
using ISynergy.Filters;
using ISynergy.Helpers;
using ISynergy.Interfaces;
using ISynergy.Options;
using ISynergy.Providers;
using ISynergy.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ISynergy
{
    public abstract class BaseStartup<TDbContext, TUser> : IAsyncInitialization
        where TDbContext : DbContext
        where TUser : IdentityUser
    {
        protected abstract string ApiDisplayName { get; }

        protected const string basePathApi = "/api";
        protected const string tokenEndpointPath = "/oauth/token";
        protected const string authorizationEndpointPath = "/oauth/authorize";
        protected const string logoutEndpointPath = "/oauth/logout";
        protected const string userinfoEndpointPath = "/api/userinfo";

        protected IHostingEnvironment _environment { get; }
        protected IConfiguration _configuration { get; }
        protected IMemoryCache _cache { get; }
        protected CultureInfo _culture { get; }

        public BaseStartup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Argument.IsNotNull(nameof(environment), environment);
            Argument.IsNotNull(nameof(configuration), configuration);

            _environment = environment;
            _configuration = configuration;
        }

        public Task Initialization { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            AddLocalization(services);
            AddOptions(services);
            AddCaching(services);
            AddDbServices(services);
            AddDataProtectionService(services);
            AddAuthentication(services);
            AddAuthorization(services);
            AddMultiTenancy(services);
            AddMultiTenantActionFilter(services);
            AddMessageService(services);
            AddServices(services);
            AddIdentity(services);
            AddLogging(services);
            AddManagers(services);
            AddMappers(services);
            AddPaymentClient(services);
            AddCloudStorage(services);
            AddSignalR(services);
            AddMvc(services);
            AddRouting(services);
            AddSwaggerGeneration(services);
            AddTelemetry(services);
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        protected virtual void AddSignalR(IServiceCollection services)
        {
            //services.Configure<PubNubSettings>(_configurationRoot.GetSection(nameof(PubNubSettings)).BindWithReload);
            //services.AddScoped<IPubNubService, PubNubService>();

            //services.AddConnections();
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            })
            .AddMessagePackProtocol();
        }

        protected virtual void AddCloudStorage(IServiceCollection services)
        {
            //services.Configure<AzureDocumentSetting>(_configurationRoot.GetSection(nameof(AzureDocumentSetting)).BindWithReload);
            //services.AddScoped<ICloudStorageService, CloudStorageService>();
        }

        protected virtual void AddOptions(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<EnviromentalSettings>(_configuration.GetSection(nameof(EnviromentalSettings)).BindWithReload);
            services.Configure<MessageServiceOptions>(_configuration.GetSection(nameof(MessageServiceOptions)).BindWithReload);
            services.Configure<Websites>(_configuration.GetSection(nameof(Websites)).BindWithReload);
            services.Configure<AuthMessageSenderOptions>(_configuration);
        }

        protected virtual void AddLocalization(IServiceCollection services)
        {
            services.AddLocalization();
        }

        protected virtual void AddCaching(IServiceCollection services)
        {
            services.AddMemoryCache();
        }

        protected virtual void AddDataProtectionService(IServiceCollection services)
        {
            services.AddDataProtection();
        }

        protected virtual void AddAuthentication(IServiceCollection services)
        {
            services.AddSingleton<IAuthenticationSchemeProvider, MultipleAuthenticationSchemeProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ClaimsAccessor>();

            // Register the OpenIddict services.
            services.AddOpenIddict()

                // Register the OpenIddict core services.
                .AddCore(options =>
                {
                    // Register the Entity Framework stores.
                    options.UseEntityFrameworkCore()
                        .UseDbContext<TDbContext>();
                })

                // Register the OpenIddict server handler.
                .AddServer(options =>
                {
                    // Register the ASP.NET Core MVC binder used by OpenIddict.
                    // Note: if you don't call this method, you won't be able to
                    // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
                    options.UseMvc();

                    options.RegisterScopes(
                        OpenIdConnectConstants.Scopes.OpenId,
                        OpenIdConnectConstants.Scopes.Email,
                        OpenIdConnectConstants.Scopes.Profile,
                        OpenIdConnectConstants.Scopes.OfflineAccess,
                        OpenIddictConstants.Scopes.Roles);

                    // Enable the token endpoint.
                    options
                        .EnableTokenEndpoint(tokenEndpointPath)
                        .EnableLogoutEndpoint(logoutEndpointPath)
                        .EnableUserinfoEndpoint(userinfoEndpointPath)
                        .EnableAuthorizationEndpoint(authorizationEndpointPath);

                    // Enable the password flow.
                    options
                        .AllowPasswordFlow()
                        .AllowRefreshTokenFlow()
                        .AllowAuthorizationCodeFlow()
                        .AllowClientCredentialsFlow();

                    // Set lifetimes of token
                    options
                        .SetAccessTokenLifetime(TimeSpan.FromDays(1))
                        .SetIdentityTokenLifetime(TimeSpan.FromDays(1))
                        .SetRefreshTokenLifetime(TimeSpan.FromDays(14));

                    // When request caching is enabled, authorization and logout requests
                    // are stored in the distributed cache by OpenIddict and the user agent
                    // is redirected to the same page with a single parameter (request_id).
                    // This allows flowing large OpenID Connect requests even when using
                    // an external authentication provider like Google, Facebook or Twitter.
                    //options.EnableRequestCaching();
                });

            // Register the OpenIddict validation handler.
            // Note: the OpenIddict validation handler is only compatible with the
            // default token format or with reference tokens and cannot be used with
            // JWT tokens. For JWT tokens, use the Microsoft JWT bearer handler.
            //.AddValidation();
            services.AddAuthentication()
                .AddOAuthValidation();
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

        protected virtual void AddMultiTenancy(IServiceCollection services)
        {
            services.AddSingleton<ITenantService, TenantService>();
        }

        protected virtual void AddMultiTenantActionFilter(IServiceCollection services)
        {
            //services.AddScoped(typeof(TenantActionFilter));
        }

        protected virtual void AddMessageService(IServiceCollection services)
        {
            //services.AddScoped<IEmailSender, MessageService>();
            //services.AddScoped<ISmsSender, MessageService>();
        }

        protected virtual void AddServices(IServiceCollection services)
        {
            services.AddScoped<IFactoryService, FactoryService>();
        }

        protected virtual void AddDbServices(IServiceCollection services)
        {
            string DataConnection = _configuration.GetConnectionString("ConnectionString");

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<TDbContext>(options =>
                {
                    options.UseSqlServer(DataConnection,
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            // Configuring Connection Resilience
                            // Doesn't work because of transactions!!!
                            // sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });

                    // Register the entity sets needed by OpenIddict.
                    // Note: use the generic overload if you need
                    // to replace the default OpenIddict entities.
                    options.UseOpenIddict();
                });
        }

        protected virtual void AddIdentity(IServiceCollection services)
        {
            // Register the Identity services.
            services.AddIdentity<TUser, IdentityRole>(options =>
            {
                options.Lockout = new LockoutOptions()
                {
                    AllowedForNewUsers = true,
                    MaxFailedAccessAttempts = 5,
                    DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15)
                };
                options.Password = new PasswordOptions()
                {
                    // (?=^.{8,}$)
                    // at least 8 characters long
                    // (?=[^\d]*\d)
                    // at least one digit
                    // (?=[^\W]*\W)
                    // at least one non-Word-Character (=one special character)
                    // (?=[^A-Z]*[A-Z])
                    // at least one uppercase character
                    // (?=[^a-z]*[a-z])
                    // at least one lowercase character
                    //RequiredRegexMatch = new Regex(Constants.PasswordRegEx),

                    RequireDigit = true,
                    RequiredLength = 6,
                    RequireLowercase = true,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = true
                };
                options.SignIn = new SignInOptions()
                {
                    RequireConfirmedEmail = true,
                    RequireConfirmedPhoneNumber = false
                };
                options.User = new UserOptions()
                {
                    // AllowedUserNameCharacters
                    RequireUniqueEmail = true
                };
            })
                .AddEntityFrameworkStores<TDbContext>()
                .AddDefaultTokenProviders();

            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });
        }

        protected virtual void AddLogging(IServiceCollection services)
        {
        }

        protected abstract void AddManagers(IServiceCollection services);
        protected abstract void AddMappers(IServiceCollection services);

        protected virtual void AddPaymentClient(IServiceCollection services)
        {
            //services.Configure<PaymentSettings>(_configurationRoot.GetSection(nameof(PaymentSettings)).BindWithReload);

            //var paymentSettings = new PaymentSettings();
            //_configurationRoot.GetSection(nameof(PaymentSettings)).Bind(paymentSettings);

            //services.AddScoped<IPaymentClient>(i => new PaymentClient(paymentSettings.Mollie_Key));
            //services.AddScoped<ICustomerClient>(i => new CustomerClient(paymentSettings.Mollie_Key));
            //services.AddScoped<IMandateClient>(i => new MandateClient(paymentSettings.Mollie_Key));
            //services.AddScoped<IRefundClient>(i => new RefundClient(paymentSettings.Mollie_Key));
            //services.AddScoped<ISubscriptionClient>(i => new SubscriptionClient(paymentSettings.Mollie_Key));
        }

        protected virtual void AddMvc(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .AddJsonOptions(options =>
                {
                    var jsonSettings = options.SerializerSettings;

                    jsonSettings.Formatting = Formatting.None;
                    jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                    // Don't include null objects in Json, but only for production environments.
                    jsonSettings.NullValueHandling =
                        !_environment.IsDevelopment()
                            ? NullValueHandling.Ignore
                            : NullValueHandling.Include;

                    // Treat datetime as unspecified in Json serializer.
                    // This means that there will be no offset information (Z/+00:00) in the output Json.
                    jsonSettings.DateFormatString = Constants.DateTimeOffsetFormat;
                    jsonSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    jsonSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                    jsonSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

                    // Serialize enums as string, instead of by their integer value.
                    var enumConverter = new Newtonsoft.Json.Converters.StringEnumConverter();
                    jsonSettings.Converters.Add(enumConverter);
                });

        }

        protected virtual void AddRouting(IServiceCollection services)
        {
            services.AddRouting(options =>
            {
                // Auto add trailing slash to url's. This is a workaround for, for example, providing emails in the url.
                // A request won't get handled if it has a period (.) in the parameters, unless there's a trailing slash at the end.
                // /api/controller/test@user.com <- DOES NOT WORK
                // /api/controller/test@user.com/ <- DOES WORK
                options.AppendTrailingSlash = true;
            });
        }

        protected virtual void AddSwaggerGeneration(IServiceCollection services)
        {
            if (_environment.IsDevelopment())
            {
                services.AddSwaggerGen(options =>
                {
                    options.CustomSchemaIds(c => c.FullName);
                    options.SwaggerDoc("v1", new Info
                    {
                        Title = ApiDisplayName,
                        Version = "v1"
                    });

                    var xmlDocPath = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, ".xml");
                    options.IncludeXmlComments(xmlDocPath);
                    options.OperationFilter<AuthorizeCheckOperationFilter>();
                    options.AddSecurityDefinition(Constants.SecuritySchemeKey, new OAuth2Scheme
                    {
                        Type = "oauth2",
                        Flow = "password",
                        AuthorizationUrl = authorizationEndpointPath,
                        TokenUrl = tokenEndpointPath,
                        Scopes = new Dictionary<string, string>()
                        {
                            { "openid", "openid" },
                            { "offline_access", "offline_access" }
                        }
                    });
                });
            }
        }

        protected virtual void AddTelemetry(IServiceCollection services)
        {
            // Configure SnapshotCollector from application settings
            //services.Configure<SnapshotCollectorConfiguration>(_configuration.GetSection(nameof(SnapshotCollectorConfiguration)));

            // Add SnapshotCollector telemetry processor.
            //services.AddSingleton<ITelemetryInitializer, UserInfoTelemetryInitializer>();
            //services.AddSingleton<ITelemetryProcessorFactory, SnapshotCollectorTelemetryProcessorFactory>();
        }

        protected async Task UpdateOpenIddictTablesAsync(TDbContext context)
        {
            context.Database.SetCommandTimeout(TimeSpan.FromMinutes(3));
            await context.Database.EnsureCreatedAsync();

            // If you use a different entity type or a custom key,
            // change this line (e.g OpenIddictApplication<long>).
            foreach (var application in context.Set<OpenIddictApplication>())
            {
                // Convert the space-separated PostLogoutRedirectUris property to JSON.
                if (!string.IsNullOrEmpty(application.PostLogoutRedirectUris) &&
                     application.PostLogoutRedirectUris[0] != '[')
                {
                    var addresses = application.PostLogoutRedirectUris.Split(
                        new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    application.PostLogoutRedirectUris =
                        new JArray(addresses).ToString(Formatting.None);
                }

                // Convert the space-separated RedirectUris property to JSON.
                if (!string.IsNullOrEmpty(application.RedirectUris) &&
                     application.RedirectUris[0] != '[')
                {
                    var addresses = application.RedirectUris.Split(
                        new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    application.RedirectUris = new JArray(addresses).ToString(Formatting.None);
                }
            }

            // If you use a different entity type or a custom key,
            // change this line (e.g OpenIddictAuthorization<long>).
            foreach (var authorization in context.Set<OpenIddictAuthorization>())
            {
                // Convert the space-separated Scopes property to JSON.
                if (!string.IsNullOrEmpty(authorization.Scopes) && authorization.Scopes[0] != '[')
                {
                    var scopes = authorization.Scopes.Split(
                        new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    authorization.Scopes = new JArray(scopes).ToString(Formatting.None);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
