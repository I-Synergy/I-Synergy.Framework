using Sample.Abstractions;


#if WINDOWS || ANDROID || IOS || MACCATALYST
using Windows.Security.Credentials;
#endif

namespace Sample.Services;

internal class CredentialLockerService : ICredentialLockerService
{
#if WINDOWS || ANDROID || IOS || MACCATALYST
    private readonly PasswordVault _vault;
    private const string _resource = "Sample";

    public CredentialLockerService()
    {
        _vault = new PasswordVault();
    }

    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        string result = string.Empty;

        try
        {
            IReadOnlyList<PasswordCredential> credentials = _vault.FindAllByResource(_resource);

            if (credentials.Count > 0)
                result = _vault.Retrieve(_resource, username)?.Password;
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
            IReadOnlyList<PasswordCredential> credentials = _vault.FindAllByResource(_resource);
            return Task.FromResult(credentials.Select(q => q.UserName).ToList());
        }
        catch (Exception)
        {
            return Task.FromResult(new List<string>());
        }
    }

    public async Task AddCredentialToCredentialLockerAsync(string username, string password)
    {
        string oldPassword = await GetPasswordFromCredentialLockerAsync(username);

        if (oldPassword != password)
        {
            await RemoveCredentialFromCredentialLockerAsync(username);

            try
            {
                _vault.Add(new PasswordCredential(_resource, username, password));
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
            IReadOnlyList<PasswordCredential> credentials = _vault.FindAllByResource(_resource);
            PasswordCredential credential = credentials.FirstOrDefault(q => q.UserName == username);

            if (credential is not null)
                _vault.Remove(credential);
        }
        catch (Exception)
        {
            // Do nothing here.
            // Credentials are not found.
        }

        return Task.CompletedTask;
    }
#else
    private readonly ApplicationDataContainer _localSettingsContainer;

    public CredentialLockerService()
    {
        _localSettingsContainer = ApplicationData.Current.LocalSettings;
    }

    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        string result = string.Empty;

        if ((string)_localSettingsContainer.Values["username"] == username)
            result = (string)_localSettingsContainer.Values["password"];

        return Task.FromResult(result);
    }

    public Task<List<string>> GetUsernamesFromCredentialLockerAsync()
    {
        List<string> result =
        [
            (string)_localSettingsContainer.Values["username"]
        ];

        return Task.FromResult(result);
    }

    public Task AddCredentialToCredentialLockerAsync(string username, string password)
    {
        _localSettingsContainer.Values["username"] = username;
        _localSettingsContainer.Values["password"] = password;

        return Task.CompletedTask;
    }

    public Task RemoveCredentialFromCredentialLockerAsync(string username)
    {
        _localSettingsContainer.Values["username"] = string.Empty;
        _localSettingsContainer.Values["password"] = string.Empty;

        return Task.CompletedTask;
    }
#endif
}
