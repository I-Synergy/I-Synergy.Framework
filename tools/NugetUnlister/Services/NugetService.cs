using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using NugetUnlister.Abstractions;
using NugetUnlister.Models;
using NugetUnlister.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NugetUnlister.Services;

internal class NugetService : INugetService
{
    private readonly IFlurlClient _client;
    private readonly NugetOptions _nugetOptions;

    public NugetService(IFlurlClient client, IOptions<NugetOptions> options)
    {
        _client = client;
        _nugetOptions = options.Value;
    }

    public async Task<NugetResponse> GetIndexAsync(string packageId, CancellationToken cancellationToken = default)
    {
        return await new Url($"https://api.nuget.org/v3-flatcontainer/{packageId.ToLowerInvariant()}/index.json")
               .WithClient(_client)
               .GetJsonAsync<NugetResponse>(cancellationToken);
    }

    public async Task<List<PackageVersion>> ListVersionAsync(string packageId, CancellationToken cancellationToken = default)
    {
        var result = new List<PackageVersion>();
        var response = await GetIndexAsync(packageId, cancellationToken);

        foreach (var version in response.Versions)
        {
            result.Add(new PackageVersion(packageId, version, true));
        }

        return result;
    }

    public Task UnlistPackageAsync(string packageId, string version, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nuget.exe");
                process.StartInfo.Arguments = $"delete {packageId} {version} -src https://api.nuget.org/v3/index.json  -apikey {_nugetOptions.ApiKey} -NonInteractive";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                StreamWriter streamWriter = process.StandardInput;
                StreamReader outputReader = process.StandardOutput;
                StreamReader errorReader = process.StandardError;
                while (!outputReader.EndOfStream)
                {
                    string text = outputReader.ReadLine();
                    streamWriter.WriteLine(text);
                }

                while (!errorReader.EndOfStream)
                {
                    string text = errorReader.ReadLine();
                    streamWriter.WriteLine(text);
                }

                streamWriter.Close();
                process.WaitForExit();
            }
        });
    }
}
