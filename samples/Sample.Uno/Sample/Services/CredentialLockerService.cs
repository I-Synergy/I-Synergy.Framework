using Sample.Abstractions;

#if WINDOWS || ANDROID || IOS
using Windows.Security.Credentials;
#endif

namespace Sample.Services;

public class CredentialLockerService : ICredentialLockerService
{
    private const string _resource = "Sample";


    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        string result = string.Empty;

#if WINDOWS || ANDROID || IOS
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
#else
#endif

        return Task.FromResult(result);
    }

    public Task<List<string>> GetUsernamesFromCredentialLockerAsync()
    {
        List<string> result = new List<string>();

#if WINDOWS || ANDROID || IOS
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
#else
#endif

        return Task.FromResult(result);
    }

#if WINDOWS || ANDROID || IOS
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
#else
    public Task AddCredentialToCredentialLockerAsync(string username, string password) => throw new NotImplementedException();
#endif

    public Task RemoveCredentialFromCredentialLockerAsync(string username)
    {
#if WINDOWS || ANDROID || IOS
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
#else
#endif

        return Task.CompletedTask;
    }
}
