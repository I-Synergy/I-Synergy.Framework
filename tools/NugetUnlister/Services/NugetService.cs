using Microsoft.Extensions.Options;
using NugetUnlister.Abstractions;
using NugetUnlister.Models;
using NugetUnlister.Options;
using System.Diagnostics;
using System.Net.Http.Json;

namespace NugetUnlister.Services;

internal class NugetService(IOptions<NugetOptions> options) : INugetService
{
    private readonly NugetOptions _nugetOptions = options.Value;

    public async Task<NugetResponse> GetIndexAsync(string packageId, CancellationToken cancellationToken = default)
    {
        using HttpClient client = new HttpClient();
        Uri url = new Uri($"https://api.nuget.org/v3-flatcontainer/{packageId.ToLowerInvariant()}/index.json");
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        HttpResponseMessage response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return default;

            return await response.Content.ReadFromJsonAsync<NugetResponse>(cancellationToken: cancellationToken);
        }
        else
            return default;
    }

    public async Task<List<PackageVersion>> ListVersionAsync(string packageId, CancellationToken cancellationToken = default)
    {
        List<PackageVersion> result = new List<PackageVersion>();
        NugetResponse response = await GetIndexAsync(packageId, cancellationToken);

        foreach (string version in response.Versions)
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
