using ISynergy.Framework.AspNetCore.WebDav.Context;
using ISynergy.Framework.AspNetCore.WebDav.Filters;
using ISynergy.Framework.AspNetCore.WebDav.Filters.Internal;
using ISynergy.Framework.AspNetCore.WebDav.Formatters.Internal;
using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.Dispatchers;
using ISynergy.Framework.AspNetCore.WebDav.Server.Engines.Remote;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Formatters;
using ISynergy.Framework.AspNetCore.WebDav.Server.Handlers;
using ISynergy.Framework.AspNetCore.WebDav.Server.Locking;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Store;
using ISynergy.Framework.AspNetCore.WebDav.Server.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.WebDav.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="IServiceCollection"/>
    /// </summary>
    public static class WebDavServicesExtensions
    {
        /// <summary>
        /// Adds the WebDAV services that are essential to run a WebDAV server.
        /// </summary>
        /// <remarks>
        /// The user must still add the following services:
        /// <list type="bullet">
        /// <item>
        ///     <term><see cref="IFileSystemFactory"/></term>
        ///     <description>The file system factory</description>
        /// </item>
        /// <item>
        ///     <term><see cref="ISynergy.Framework.AspNetCore.WebDav.Server.Props.Store.IPropertyStoreFactory"/></term>
        ///     <description>The property store factory</description>
        /// </item>
        /// <item>
        ///     <term><see cref="ILockManager"/></term>
        ///     <description>The lock manager</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="services">The service collection to add the WebDAV services to</param>
        /// <returns>the <paramref name="services"/></returns>
        public static IServiceCollection AddWebDav(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, WebDavXmlSerializerMvcOptionsSetup>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, WebDavExceptionFilterMvcOptionsSetup>());
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddScoped<IDeadPropertyFactory, DeadPropertyFactory>();
            services.TryAddScoped<IRemoteCopyTargetActionsFactory, DefaultRemoteTargetActionsFactory>();
            services.TryAddScoped<IRemoteMoveTargetActionsFactory, DefaultRemoteTargetActionsFactory>();
            services.TryAddSingleton<IHttpMessageHandlerFactory, DefaultHttpMessageHandlerFactory>();
            services.TryAddSingleton<ISystemClock, SystemClock>();
            services.TryAddSingleton<ITimeoutPolicy, DefaultTimeoutPolicy>();
            services.TryAddScoped<IWebDavContext, WebDavContext>();
            services.TryAddSingleton<ILockCleanupTask, LockCleanupTask>();
            services.TryAddSingleton<IPathTraversalEngine, PathTraversalEngine>();
            services.TryAddSingleton<IMimeTypeDetector, DefaultMimeTypeDetector>();
            services.TryAddSingleton<IEntryPropertyInitializer, DefaultEntryPropertyInitializer>();
            services
                .AddOptions()
                .AddScoped<IWebDavDispatcher, WebDavServer>()
                .AddSingleton<WebDavExceptionFilter>()
                .AddScoped<IWebDavOutputFormatter, WebDavXmlOutputFormatter>()
                .AddSingleton<LockCleanupTask>();
            services.Scan(
                scan => scan
                    .FromAssemblyOf<IHandler>()
                    .AddClasses(classes => classes.AssignableToAny(typeof(IHandler), typeof(IWebDavClass)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());
            services.AddScoped(
                sp =>
                {
                    var factory = sp.GetRequiredService<IFileSystemFactory>();
                    var context = sp.GetRequiredService<IWebDavContext>();
                    return factory.CreateFileSystem(null, context.User);
                });
            services.AddScoped(
                sp =>
                {
                    var factory = sp.GetRequiredService<IPropertyStoreFactory>();
                    var fileSystem = sp.GetRequiredService<IFileSystem>();
                    return factory.Create(fileSystem);
                });
            return services;
        }
    }
}
