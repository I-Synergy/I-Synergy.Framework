using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Mvvm.Abstractions.Services;
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
    private readonly IInfoService _infoService;

    public CredentialLockerService(IInfoService infoService)
    {
        _infoService = infoService;
    }

    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        return Task.FromResult(TryCatchUtility.IgnoreErrors<string, COMException>(() =>
        {
            PasswordVault vault = new();
            IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(_infoService.ProductName);

            if (credentials.Count > 0)
                return vault.Retrieve(_infoService.ProductName, username)?.Password;

            return string.Empty;
        }));
    }

    public Task<List<string>> GetUsernamesFromCredentialLockerAsync()
    {
        return Task.FromResult(TryCatchUtility.IgnoreErrors<List<string>, COMException>(() =>
        {
            PasswordVault vault = new();
            IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(_infoService.ProductName);
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
                vault.Add(new PasswordCredential(_infoService.ProductName, username, password));
            }
        });

        return Task.CompletedTask;
    }

    public Task RemoveCredentialFromCredentialLockerAsync(string username)
    {
        TryCatchUtility.IgnoreErrors<COMException>(() =>
        {
            PasswordVault vault = new();
            IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(_infoService.ProductName);
            PasswordCredential credential = credentials.FirstOrDefault(q => q.UserName == username);

            if (credential is not null)
                vault.Remove(credential);
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