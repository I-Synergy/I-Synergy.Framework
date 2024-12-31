using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using Windows.Security.Credentials;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// CredentialLockerService constructor
/// </summary>
public class CredentialLockerService : ICredentialLockerService
{
    private readonly ILogger _logger;

    public CredentialLockerService(ILogger<CredentialLockerService> logger)
    {
        _logger = logger;
        _logger.LogDebug($"CredentialLockerService instance created with ID: {Guid.NewGuid()}");
    }

    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        return Task.FromResult(TryCatchUtility.IgnoreErrors<string, COMException>(() =>
        {
            PasswordVault vault = new();
            IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(InfoService.Default.ProductName);

            if (credentials.Count > 0)
                return vault.Retrieve(InfoService.Default.ProductName, username)?.Password;

            return string.Empty;
        }));
    }

    public Task<List<string>> GetUsernamesFromCredentialLockerAsync()
    {
        return Task.FromResult(TryCatchUtility.IgnoreErrors<List<string>, COMException>(() =>
        {
            PasswordVault vault = new();
            IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(InfoService.Default.ProductName);
            return credentials.Select(q => q.UserName).ToList();
        }));
    }

    public Task AddCredentialToCredentialLockerAsync(string username, string password)
    {
        TryCatchUtility.IgnoreErrors<COMException>(async () =>
        {
            PasswordVault vault = new();
            string oldPassword = await GetPasswordFromCredentialLockerAsync(username);

            if (oldPassword != password)
            {
                await RemoveCredentialFromCredentialLockerAsync(username);
                vault.Add(new PasswordCredential(InfoService.Default.ProductName, username, password));
            }
        });

        return Task.CompletedTask;
    }

    public Task RemoveCredentialFromCredentialLockerAsync(string username)
    {
        TryCatchUtility.IgnoreErrors<COMException>(() =>
        {
            PasswordVault vault = new();
            IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(InfoService.Default.ProductName);
            PasswordCredential credential = credentials.FirstOrDefault(q => q.UserName == username);

            if (credential is not null)
                vault.Remove(credential);
        });

        return Task.CompletedTask;
    }
}