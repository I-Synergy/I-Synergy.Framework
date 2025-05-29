using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;


#if WINDOWS
using Windows.Security.Credentials;
#endif

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// CredentialLockerService constructor
/// </summary>
public class CredentialLockerService : ICredentialLockerService
{
#if WINDOWS
    private readonly ILogger _logger;
    private readonly IInfoService _infoService;

    public CredentialLockerService(IInfoService infoService, ILogger<CredentialLockerService> logger)
    {
        _infoService = infoService;
        _logger = logger;
        _logger.LogTrace($"CredentialLockerService instance created with ID: {Guid.NewGuid()}");
    }

    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        return Task.FromResult(TryCatchUtility.IgnoreErrors<string, COMException>(
        () =>
        {
            try
            {
                PasswordVault vault = new();
                IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(_infoService.ProductName);

                if (credentials.Count > 0)
                    return vault.Retrieve(_infoService.ProductName, username).Password;

                return string.Empty;
            }
            catch (COMException ex) when (ex.HResult == unchecked((int)0x80070490))
            {
                return string.Empty;
            }
        },
        string.Empty)!);
    }

    public Task<List<string>> GetUsernamesFromCredentialLockerAsync()
    {
        return Task.FromResult(TryCatchUtility.IgnoreErrors<List<string>, COMException>(
        () =>
        {
            try
            {
                var vault = new PasswordVault();
                var credentials = vault.FindAllByResource(_infoService.ProductName);
                return credentials.Select(q => q.UserName).ToList();
            }
            catch (COMException ex) when (ex.HResult == unchecked((int)0x80070490))
            {
                return new List<string>();
            }
        },
        new List<string>())!);
    }

    public Task AddCredentialToCredentialLockerAsync(string username, string password)
    {
        TryCatchUtility.IgnoreErrors<COMException>(async () =>
        {
            var vault = new PasswordVault();

            // Use nested try-catch for handling the specific "not found" case
            try
            {
                var oldPassword = await GetPasswordFromCredentialLockerAsync(username);
                if (oldPassword != password)
                {
                    await RemoveCredentialFromCredentialLockerAsync(username);
                    vault.Add(new PasswordCredential(_infoService.ProductName, username, password));
                }
            }
            catch (COMException ex) when (ex.HResult == unchecked((int)0x80070490))
            {
                // No existing credential found, just add new one
                vault.Add(new PasswordCredential(_infoService.ProductName, username, password));
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
                var credentials = vault.FindAllByResource(_infoService.ProductName);
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
#else
    public async Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        string result = string.Empty;

        if (await SecureStorage.Default.GetAsync("username") == username)
            result = await SecureStorage.Default.GetAsync("password");

        return result;
    }

    public async Task<List<string>> GetUsernamesFromCredentialLockerAsync()
    {
        List<string> result =
        [
            await SecureStorage.Default.GetAsync("username")
        ];
        return result;
    }

    public async Task AddCredentialToCredentialLockerAsync(string username, string password)
    {
        await SecureStorage.Default.SetAsync("username", username);
        await SecureStorage.Default.SetAsync("password", password);
    }

    public Task RemoveCredentialFromCredentialLockerAsync(string username)
    {
        SecureStorage.Default.Remove("username");
        SecureStorage.Default.Remove("password");
        return Task.CompletedTask;
    }
#endif
}