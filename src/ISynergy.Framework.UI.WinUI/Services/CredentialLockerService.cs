using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Windows.Security.Credentials;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// CredentialLockerService constructor
/// </summary>
public class CredentialLockerService : ICredentialLockerService
{
    private readonly IInfoService _infoService;

    public CredentialLockerService(IInfoService infoService)
    {
        _infoService = infoService;
    }

    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        string result = string.Empty;

        try
        {
            PasswordVault vault = new();
            IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(_infoService.ProductName);

            if (credentials.Count > 0)
                result = vault.Retrieve(_infoService.ProductName, username)?.Password;
        }
        catch (Exception)
        {
            result = null;
        }

        return Task.FromResult(result);
    }

    public Task<List<string>> GetUsernamesFromCredentialLockerAsync()
    {
        List<string> result = [];

        try
        {
            PasswordVault vault = new();
            IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(_infoService.ProductName);
            result = credentials.Select(q => q.UserName).ToList();
        }
        catch (Exception)
        {
            result = null;
        }

        return Task.FromResult(result);
    }

    public async Task AddCredentialToCredentialLockerAsync(string username, string password)
    {
        PasswordVault vault = new();
        string oldPassword = await GetPasswordFromCredentialLockerAsync(username);

        if (oldPassword != password)
        {
            await RemoveCredentialFromCredentialLockerAsync(username);

            try
            {
                vault.Add(new PasswordCredential(_infoService.ProductName, username, password));
            }
            catch (Exception)
            {
                // Do nothing here...
            }
        }
    }

    public Task RemoveCredentialFromCredentialLockerAsync(string username)
    {
        try
        {
            PasswordVault vault = new();
            IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(_infoService.ProductName);
            PasswordCredential credential = credentials.FirstOrDefault(q => q.UserName == username);

            if (credential is not null)
                vault.Remove(credential);
        }
        catch (Exception)
        {
            // Do nothing here.
            // Credentials are not found.
        }

        return Task.CompletedTask;
    }
}