using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Base;
using System.ComponentModel;
using System.Reflection;

namespace ISynergy.Framework.Core.Services;

/// <summary>
/// Class BaseInfoService.
/// </summary>
[Bindable(BindableSupport.Yes)]
public sealed class InfoService : ObservableClass, IInfoService
{
    private static readonly object _creationLock = new object();
    private static IInfoService? _defaultInstance;

    /// <summary>
    /// Gets the LanguageService's default instance.
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

    public InfoService()
    {
        ProductVersion = new Version("0.0.0");
        ApplicationPath = string.Empty;
        CompanyName = string.Empty;
        ProductName = string.Empty;
        Copyrights = string.Empty;
    }

    /// <summary>
    /// Gets the product version.
    /// </summary>
    /// <value>The product version.</value>
    public Version ProductVersion
    {
        get { return GetValue<Version>(); }
        private set { SetValue(value); }
    }

    /// <summary>
    /// Gets the application path.
    /// </summary>
    /// <value>The application path.</value>
    public string ApplicationPath
    {
        get { return GetValue<string>(); }
        private set { SetValue(value); }
    }

    /// <summary>
    /// Gets the name of the company.
    /// </summary>
    /// <value>The name of the company.</value>
    public string CompanyName
    {
        get { return GetValue<string>(); }
        private set { SetValue(value); }
    }

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    /// <value>The name of the product.</value>
    public string ProductName
    {
        get { return GetValue<string>(); }
        private set { SetValue(value); }
    }

    /// <summary>
    /// Gets the copy rights detail.
    /// </summary>
    /// <value>The copy rights detail.</value>
    public string Copyrights
    {
        get { return GetValue<string>(); }
        private set { SetValue(value); }
    }

    /// <summary>
    /// Loads the assembly into the Version service.
    /// </summary>
    /// <param name="assembly"></param>
    public void LoadAssembly(Assembly assembly)
    {
        ApplicationPath = System.AppContext.BaseDirectory;
        CompanyName = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;
        ProductName = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? string.Empty;
        Copyrights = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? string.Empty;

        if (assembly.IsDefined(typeof(AssemblyInformationalVersionAttribute), false) &&
            Version.TryParse(assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion, out var informationalVersion))
        {
            ProductVersion = informationalVersion;
        }
        else if (assembly.IsDefined(typeof(AssemblyVersionAttribute), false) &&
            Version.TryParse(assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version, out var assemblyVersion))
        {
            ProductVersion = assemblyVersion;
        }
        else if (assembly.IsDefined(typeof(AssemblyFileVersionAttribute), false) &&
            Version.TryParse(assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version, out var fileVersion))
        {
            ProductVersion = fileVersion;
        }
        else
        {
            ProductVersion = new Version("0.0.0");
        }
    }
}
