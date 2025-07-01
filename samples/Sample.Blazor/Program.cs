using ISynergy.Framework.AspNetCore.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.OpenTelemetry.Extensions;
using ISynergy.Framework.UI.Extensions;
using Sample.Components;
using Sample.Models;
using Sample.Services;
using System.Reflection;

namespace Sample;
public class Program
{
    public static void Main(string[] args)
    {
        var mainAssembly = Assembly.GetExecutingAssembly();
        var infoService = new InfoService();
        infoService.LoadAssembly(mainAssembly);

        var builder = WebApplication.CreateBuilder(args);

        builder.ConfigureServices<Context, CommonServices, AuthenticationService, SettingsService<LocalSettings, RoamingSettings, GlobalSettings>, Properties.Resources>(
            infoService,
            (builder) =>
            {
                builder.Logging
                    .AddTelemetry(builder, infoService)
                    .AddOtlpExporter();
            },
            mainAssembly,
            f => f.Name!.StartsWith(typeof(Program).Namespace!));


        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder
            .Build()
            .SetLocatorProvider();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
