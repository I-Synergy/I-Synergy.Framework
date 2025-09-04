namespace ISynergy.Framework.UI.Abstractions.Services;

public interface ICredentialLockerService
{
    /// <summary>
    /// Gets all usernames in the credential locker of the OS.
    /// </summary>
    /// <returns></returns>
    Task<List<string>> GetUsernamesFromCredentialLockerAsync();

    /// <summary>
    /// Retrieve password for specified user in the credential locker of the OS.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<string> GetPasswordFromCredentialLockerAsync(string username);

    /// <summary>
    /// Saves the credentials to the credential locker of the OS.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task AddCredentialToCredentialLockerAsync(string username, string password);

    /// <summary>
    /// Removes the credentials from the credential locker of the OS.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task RemoveCredentialFromCredentialLockerAsync(string username);
}
