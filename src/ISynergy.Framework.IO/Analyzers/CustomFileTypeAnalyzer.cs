using ISynergy.Framework.IO.Analyzers.Base;
using System.Text;

namespace ISynergy.Framework.IO.Analyzers;

/// <summary>
/// Class CustomFileTypeAnalyzer.
/// Implements the <see cref="BaseFileTypeAnalyzer" />
/// </summary>
/// <seealso cref="BaseFileTypeAnalyzer" />
public class CustomFileTypeAnalyzer : BaseFileTypeAnalyzer
{
    /// <summary>
    /// Initializes a <see cref="CustomFileTypeAnalyzer" /> with the provided definitions file contents.
    /// </summary>
    /// <param name="definitionsFile">The json object representing the definitions file.</param>
    public CustomFileTypeAnalyzer(string definitionsFile) // NOSONAR
        : base(definitionsFile)
    {
    }

    /// <summary>
    /// Initializes a <see cref="CustomFileTypeAnalyzer" /> with the definitions at the provided file path.
    /// </summary>
    /// <param name="filePath">Definitions file path. The path must point to an existing file.</param>
    /// <param name="encoding">The encoding.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is <c>null</c>.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the resolved file path does not exist.</exception>
    public CustomFileTypeAnalyzer(string filePath, Encoding encoding)
        : base(ReadDefinitionsFile(filePath, encoding))
    {
    }

    private static string ReadDefinitionsFile(string filePath, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        // Resolve to an absolute path to normalize any relative segments.
        // Note: this does NOT restrict which directories may be read; callers are
        // responsible for ensuring that only trusted paths are supplied.
        var resolvedPath = Path.GetFullPath(filePath);

        if (!File.Exists(resolvedPath))
            throw new FileNotFoundException("The definitions file was not found at the resolved path.", resolvedPath);

        return File.ReadAllText(resolvedPath, encoding);
    }

    /// <summary>
    /// Initializes a <see cref="CustomFileTypeAnalyzer" /> with the definitions from the provided stream.
    /// </summary>
    /// <param name="definitionStream">Definitions stream.</param>
    public CustomFileTypeAnalyzer(Stream definitionStream)
        : base(new StreamReader(definitionStream).ReadToEnd())
    {
    }
}
