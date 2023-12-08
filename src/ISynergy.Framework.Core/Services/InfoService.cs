using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using System.Reflection;

namespace ISynergy.Framework.Core.Services;

/// <summary>
/// Class BaseInfoService.
/// </summary>
public class InfoService : IInfoService
{
    private static readonly object _creationLock = new object();
    private static IInfoService _defaultInstance;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoService" /> class.
    /// </summary>
    public InfoService()
    {
    }

    /// <summary>
    /// Gets the Messenger's default instance, allowing
    /// to register and send messages in a static manner.
    /// </summary>
    public static IInfoService Default
    {
        get
        {
            if (_defaultInstance is null)
            {
                lock (_creationLock)
                {
                    if (_defaultInstance is null)
                    {
                        _defaultInstance = new InfoService();
                    }
                }
            }

            return _defaultInstance;
        }
    }

    /// <summary>
    /// Gets the product version.
    /// </summary>
    /// <value>The product version.</value>
    public Version ProductVersion { get; private set; }

    /// <summary>
    /// Gets the application path.
    /// </summary>
    /// <value>The application path.</value>
    public string ApplicationPath { get; private set; }

    /// <summary>
    /// Gets the name of the company.
    /// </summary>
    /// <value>The name of the company.</value>
    public string CompanyName { get; private set; }

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    /// <value>The name of the product.</value>
    public string ProductName { get; private set; }

    /// <summary>
    /// Gets the copy rights detail.
    /// </summary>
    /// <value>The copy rights detail.</value>
    public string Copyrights { get; private set; }

    /// <summary>
    /// Gets the application title.
    /// </summary>
    public string Title { get; private set; } 

    /// <summary>
    /// Loads the assembly into the Version service.
    /// </summary>
    /// <param name="assembly"></param>
    public void LoadAssembly(Assembly assembly)
    {
        ApplicationPath = System.AppContext.BaseDirectory;
        CompanyName = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
        ProductName = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
        Copyrights = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;

        if (assembly.IsDefined(typeof(AssemblyInformationalVersionAttribute), false) && 
            Version.TryParse(assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion, out var informationalVersion))
        {
            ProductVersion = informationalVersion;
        }
        else if (assembly.IsDefined(typeof(AssemblyVersionAttribute), false) && 
            Version.TryParse(assembly.GetCustomAttribute<AssemblyVersionAttribute>().Version, out var assemblyVersion))
        {
            ProductVersion = assemblyVersion;
        }
        else if (assembly.IsDefined(typeof(AssemblyFileVersionAttribute), false) && 
            Version.TryParse(assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version, out var fileVersion))
        {
            ProductVersion = fileVersion;
        }
        else
        {
            ProductVersion = new Version("0.0.0");
        }
    }

    public void SetTitle(SoftwareEnvironments environment)
    {
        Title = $"{ProductName} v{ProductVersion} ({environment})";
    }
}
