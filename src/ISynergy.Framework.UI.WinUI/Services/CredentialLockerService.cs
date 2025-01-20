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
        return Task.FromResult(TryCatchUtility.IgnoreErrors<string, COMException>(
        () =>
        {
            try
            {
                PasswordVault vault = new();
                IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(InfoService.Default.ProductName);

                if (credentials.Count > 0)
                    return vault.Retrieve(InfoService.Default.ProductName, username)?.Password;

                return string.Empty;
            }
            catch (COMException ex) when (ex.HResult == unchecked((int)0x80070490))
            {
                return string.Empty;
            }
        },
        string.Empty));
    }

    public Task<List<string>> GetUsernamesFromCredentialLockerAsync()
    {
        return Task.FromResult(TryCatchUtility.IgnoreErrors<List<string>, COMException>(
        () =>
        {
            try
            {
                var vault = new PasswordVault();
                var credentials = vault.FindAllByResource(InfoService.Default.ProductName);
                return credentials.Select(q => q.UserName).ToList();
            }
            catch (COMException ex) when (ex.HResult == unchecked((int)0x80070490))
            {
                return new List<string>();
            }
        },
        new List<string>()));
    }

    public Task AddCredentialToCredentialLockerAsync(string username, string password)
    {
        TryCatchUtility.IgnoreErrors<COMException>(async () =>
        {
            var vault = new PasswordVault();

            // Use nested try-catch for handling the specific "not found" case
            try
            {
                string oldPassword = await GetPasswordFromCredentialLockerAsync(username);
                if (oldPassword != password)
                {
                    await RemoveCredentialFromCredentialLockerAsync(username);
                    vault.Add(new PasswordCredential(InfoService.Default.ProductName, username, password));
                }
            }
            catch (COMException ex) when (ex.HResult == unchecked((int)0x80070490))
            {
                // No existing credential found, just add new one
                vault.Add(new PasswordCredential(InfoService.Default.ProductName, username, password));
            }
        });

        return Task.CompletedTask;
    }

    public Task RemoveCredentialFromCredentialLockerAsync(string username)
    {
        TryCatchUtility.IgnoreErrors<COMException>(() =>
        {
            var vault = new PasswordVault();

            try
            {
                var credentials = vault.FindAllByResource(InfoService.Default.ProductName);
                var credential = credentials.FirstOrDefault(q => q.UserName == username);

                if (credential is not null)
                {
                    vault.Remove(credential);
                }
            }
            catch (COMException ex) when (ex.HResult == unchecked((int)0x80070490))
            {
                // No credentials found, nothing to remove
            }
        });

        return Task.CompletedTask;
    }
}