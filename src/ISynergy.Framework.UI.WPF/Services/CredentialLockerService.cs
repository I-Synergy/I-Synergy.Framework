using ISynergy.Framework.Core.Abstractions.Services;
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
    private readonly IInfoService _infoService;

    public CredentialLockerService(IInfoService infoService, ILogger<CredentialLockerService> logger)
    {
        _infoService = infoService;
        _logger = logger;
    }

    public Task<string> GetPasswordFromCredentialLockerAsync(string username)
    {
        ISynergy.Framework.UI.Models.Credential result = CredentialManager.ReadCredential(_infoService.ProductName, username);

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
        CredentialManager.WriteCredential(_infoService.ProductName, username, password);
        return Task.CompletedTask;
    }

    public Task RemoveCredentialFromCredentialLockerAsync(string username)
    {
        CredentialManager.DeleteCredential(_infoService.ProductName);
        return Task.CompletedTask;
    }
}