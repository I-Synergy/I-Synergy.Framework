using Sample.Abstractions;

#if WINDOWS
using Windows.Security.Credentials;
#endif

namespace Sample.Services;

internal class CredentialLockerService : ICredentialLockerService
{
#if WINDOWS
    private const string _resource = "Sample";

    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        string result = string.Empty;

        try
        {
            PasswordVault vault = new PasswordVault();
            IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(_resource);

            if (credentials.Count > 0)
                result = vault.Retrieve(_resource, username)?.Password;
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
            PasswordVault vault = new PasswordVault();
            IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(_resource);
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
        PasswordVault vault = new PasswordVault();
        string oldPassword = await GetPasswordFromCredentialLockerAsync(username);

        if (oldPassword != password)
        {
            await RemoveCredentialFromCredentialLockerAsync(username);

            try
            {
                vault.Add(new PasswordCredential(_resource, username, password));
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
            PasswordVault vault = new PasswordVault();
            IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(_resource);
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
