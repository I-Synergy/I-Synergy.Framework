namespace ISynergy.Framework.Core.Enumerations;

/// <summary>
/// Enum SoftwareEnvironments
/// </summary>
public enum SoftwareEnvironments
{
    /// <summary>
    /// The production environment
    /// </summary>
    Production = 0,
    /// <summary>
    /// The testing and acceptance environment
    /// </summary>
    Test = -1,
    /// <summary>
    /// The local development environment
    /// </summary>
    Local = -2
}
