using ISynergy.Framework.AspNetCore.Blazor.Abstractions.Services;
using ISynergy.Framework.AspNetCore.Blazor.Extensions;
using ISynergy.Framework.AspNetCore.Extensions;
using ISynergy.Framework.Core.Serializers;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.OpenTelemetry.Extensions;
using Microsoft.FluentUI.AspNetCore.Components;
using Sample.Components;
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

        builder.Logging
            .AddTelemetry(builder, infoService)
            .AddOtlpExporter();

        builder.Services
            .ConfigureServices<Context, CommonServices, SettingsService, Properties.Resources>(
                builder.Configuration,
                infoService,
                services =>
                {
                    builder.Services.AddSingleton((s) => DefaultJsonSerializers.Web);

                    builder.Services.AddRazorComponents().AddInteractiveServerComponents();

                    builder.Services.AddFluentUIComponents();

                    builder.Services.AddSingleton<INavigationMenuService, NavigationMenuService>();
                },
                mainAssembly,
                f => f.Name!.StartsWith(typeof(Program).Namespace!));


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
