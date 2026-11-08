using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// WinUI half of <see cref="ActivationExtensions"/>: takes over single-instance activation and shows
/// what the secondary instance was launched with (environment, protocol and command-line arguments).
/// </summary>
public static partial class ActivationExtensions
{
    /// <summary>
    /// Enables single-instance activation and surfaces the activation arguments in dialogs.
    /// </summary>
    /// <remarks>
    /// The handlers are diagnostic: they make it obvious which arguments a second instance carried, which
    /// is the usual reason a packaged app "does nothing" when launched twice.
    /// </remarks>
    /// <param name="appBuilder">The MAUI app builder.</param>
    /// <returns>The same builder.</returns>
    public static MauiAppBuilder ConfigureActivationDiagnostics(this MauiAppBuilder appBuilder)
    {
        ArgumentNullException.ThrowIfNull(appBuilder);

        appBuilder
            .ConfigureSingleInstance()
            .ConfigureActivationHandlers(
                async arguments =>
                {
                    foreach (var argument in arguments.EnsureNotNull())
                    {
                        if (!string.IsNullOrEmpty(argument))
                        {
                            await IPlatformApplication.Current!.Services
                                .GetRequiredService<IDialogService>()
                                .ShowMessageAsync(argument, "Environment");
                        }
                    }
                },
                async protocol =>
                {
                    if (protocol.Uri is null)
                        return;

                    var queryArgs = protocol.Uri.Query.ParseQueryString();

                    // Skip --environment / -e equivalents - already consumed at startup.
                    queryArgs.Remove("environment");
                    queryArgs.Remove("e");

                    var parts = new List<string>();

                    if (!string.IsNullOrEmpty(protocol.Uri.Host))
                        parts.Add($"Host: {protocol.Uri.Host}");

                    if (!string.IsNullOrEmpty(protocol.Uri.AbsolutePath) && protocol.Uri.AbsolutePath != "/")
                        parts.Add($"Path: {protocol.Uri.AbsolutePath}");

                    if (queryArgs.Count > 0)
                        parts.AddRange(queryArgs.Select(kvp => $"  {kvp.Key} = {kvp.Value}"));

                    if (parts.Count == 0)
                        return;

                    await IPlatformApplication.Current!.Services
                        .GetRequiredService<IDialogService>()
                        .ShowMessageAsync(string.Join(Environment.NewLine, parts), "Protocol Activation");
                },
                async launch =>
                {
                    if (string.IsNullOrEmpty(launch.Arguments))
                        return;

                    var args = launch.Arguments.ParseCommandLineArgs();

                    // Skip --environment / -e - already consumed at startup.
                    args.Remove("environment");
                    args.Remove("e");

                    if (args.Count == 0)
                        return;

                    var message = string.Join(
                        Environment.NewLine,
                        args.Select(kvp => $"--{kvp.Key} {kvp.Value}"));

                    await IPlatformApplication.Current!.Services
                        .GetRequiredService<IDialogService>()
                        .ShowMessageAsync(message, "Launch Arguments");
                });

        return appBuilder;
    }
}
