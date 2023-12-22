using ISynergy.Framework.UI.Win32;
using Sample.Abstractions;

namespace Sample.Services;

internal class CredentialLockerService : ICredentialLockerService
{
    private const string _resource = "Sample";

    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        ISynergy.Framework.UI.Models.Credential result = CredentialManager.ReadCredential(_resource, username);

        if (result is not null)
            return Task.FromResult(result.Password);

        return Task.FromResult<string>(null);
    }

    public Task<List<string>> GetUsernamesFromCredentialLockerAsync()
    {
        IReadOnlyList<ISynergy.Framework.UI.Models.Credential> credentials = CredentialManager.EnumerateCrendentials();
        return Task.FromResult(credentials.Select(q => q.Username).ToList());
    }

    public Task AddCredentialToCredentialLockerAsync(string username, string password)
    {
        CredentialManager.WriteCredential(_resource, username, password);
        return Task.CompletedTask;
    }

    public Task RemoveCredentialFromCredentialLockerAsync(string username)
    {
        CredentialManager.DeleteCredential(_resource);
        return Task.CompletedTask;
    }
}
