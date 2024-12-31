using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Win32;
using Microsoft.Extensions.Logging;

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
    }

    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        ISynergy.Framework.UI.Models.Credential result = CredentialManager.ReadCredential(InfoService.Default.ProductName, username);

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
        CredentialManager.WriteCredential(InfoService.Default.ProductName, username, password);
        return Task.CompletedTask;
    }

    public Task RemoveCredentialFromCredentialLockerAsync(string username)
    {
        CredentialManager.DeleteCredential(InfoService.Default.ProductName);
        return Task.CompletedTask;
    }
}