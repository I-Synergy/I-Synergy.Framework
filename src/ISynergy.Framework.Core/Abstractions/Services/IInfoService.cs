using ISynergy.Framework.Core.Enumerations;
using System.Reflection;

namespace ISynergy.Framework.Core.Abstractions.Services;

/// <summary>
/// Interface IInfoService
/// </summary>
public interface IInfoService
{
    /// <summary>
    /// Gets the application path.
    /// </summary>
    /// <value>The application path.</value>
    string ApplicationPath { get; }
    /// <summary>
    /// Gets the name of the company.
    /// </summary>
    /// <value>The name of the company.</value>
    string CompanyName { get; }
    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    /// <value>The name of the product.</value>
    string ProductName { get; }
    /// <summary>
    /// Gets the copy rights.
    /// </summary>
    /// <value>The copy rights detail.</value>
    string Copyrights { get; }
    /// <summary>
    /// Gets the product version.
    /// </summary>
    /// <value>The product version.</value>
    Version ProductVersion { get; }
    /// <summary>
    /// Gets the application title.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Loads the assembly into the Version service.
    /// </summary>
    /// <param name="assembly"></param>
    void LoadAssembly(Assembly assembly);

    /// <summary>
    /// Sets the title.
    /// </summary>
    /// <param name="environment"></param>
    void SetTitle(SoftwareEnvironments environment);
}
