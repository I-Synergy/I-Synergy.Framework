using ISynergy.Framework.IO.Analyzers.Base;
using System.Reflection;

namespace ISynergy.Framework.IO.Analyzers
{
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
            : base(new StreamReader(
                Assembly.GetAssembly(typeof(FileTypeAnalyzer))
                  .GetManifestResourceStream("FileTypeDefinitions.json"))
                  .ReadToEnd())
        { }
    }
}
