using Sample.Abstractions;
using Windows.Security.Credentials;

namespace Sample.Services;

internal class CredentialLockerService : ICredentialLockerService
{
    private const string _resource = "Sample";

    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        var result = string.Empty;

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

        return Task.FromResult(result);
    }

    public Task<List<string>> GetUsernamesFromCredentialLockerAsync()
    {
        try
        {
            var vault = new PasswordVault();
            var credentials = vault.FindAllByResource(_resource);
            return Task.FromResult(credentials.Select(q => q.UserName).ToList());
        }
        catch (Exception)
        {
            return Task.FromResult(new List<string>());
        }
    }

    public async Task AddCredentialToCredentialLockerAsync(string username, string password)
    {
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
    }

    public Task RemoveCredentialFromCredentialLockerAsync(string username)
    {
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

        return Task.CompletedTask;
    }
}
