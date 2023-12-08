using Sample.Abstractions;

#if WINDOWS
using Windows.Security.Credentials;
#endif

namespace Sample.Services;

internal class CredentialLockerService : ICredentialLockerService
{
    private const string _resource = "Sample";


    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        var result = string.Empty;

#if WINDOWS
        try
        {
            var vault = new PasswordVault();
            var credentials = vault.FindAllByResource(_resource);

            if (credentials.Count > 0)
                result = vault.Retrieve(_resource, username)?.Password;
        }
        catch (Exception)
        {
            result = null;
        }
#else
        if (SecureStorage.Default.GetAsync("username").GetAwaiter().GetResult() == username)
            result = SecureStorage.Default.GetAsync("password").GetAwaiter().GetResult();
#endif

        return Task.FromResult(result);
    }

    public Task<List<string>> GetUsernamesFromCredentialLockerAsync()
    {
        var result = new List<string>();

#if WINDOWS
        try
        {
            var vault = new PasswordVault();
            var credentials = vault.FindAllByResource(_resource);
            result = credentials.Select(q => q.UserName).ToList();
        }
        catch (Exception)
        {
            result = null;
        }
#else
        result.Add(SecureStorage.Default.GetAsync("username").GetAwaiter().GetResult());
#endif

        return Task.FromResult(result);
    }

    public async Task AddCredentialToCredentialLockerAsync(string username, string password)
    {
#if WINDOWS
        var vault = new PasswordVault();
        var oldPassword = await GetPasswordFromCredentialLockerAsync(username);

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
#else
        await SecureStorage.Default.SetAsync("username", username);
        await SecureStorage.Default.SetAsync("password", password);
#endif
    }

    public Task RemoveCredentialFromCredentialLockerAsync(string username)
    {
#if WINDOWS
        try
        {
            var vault = new PasswordVault();
            var credentials = vault.FindAllByResource(_resource);
            var credential = credentials.FirstOrDefault(q => q.UserName == username);

            if (credential is not null)
                vault.Remove(credential);
        }
        catch (Exception)
        {
            // Do nothing here.
            // Credentials are not found.
        }
#else
        SecureStorage.Default.Remove("username");
        SecureStorage.Default.Remove("password");
#endif

        return Task.CompletedTask;
    }
}
