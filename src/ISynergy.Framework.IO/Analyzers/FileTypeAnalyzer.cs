using ISynergy.Framework.IO.Analyzers.Base;
using System.Reflection;

namespace ISynergy.Framework.IO.Analyzers;

/// <summary>
/// Class FileTypeAnalyzer.
/// Implements the <see cref="BaseFileTypeAnalyzer" />
/// </summary>
/// <seealso cref="BaseFileTypeAnalyzer" />
public class FileTypeAnalyzer : BaseFileTypeAnalyzer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileTypeAnalyzer" /> class.
    /// </summary>
    public FileTypeAnalyzer()
        : base(ReadEmbeddedResource())
    { }

    private static string ReadEmbeddedResource()
    {
        var assembly = Assembly.GetAssembly(typeof(FileTypeAnalyzer))
            ?? throw new InvalidOperationException("Could not get assembly for FileTypeAnalyzer");

        // Get the correct resource name - embedded resources include the namespace
        string resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith("FileTypeDefinitions.json"))
            ?? "ISynergy.Framework.IO.FileTypeDefinitions.json"; // Fallback name

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
