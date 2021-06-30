using ISynergy.Framework.IO.Base;
using System.IO;
using System.Reflection;

namespace ISynergy.Framework.IO
{
    /// <summary>
    /// Class FileTypeAnalyzer.
    /// Implements the <see cref="ISynergy.Framework.IO.Base.BaseFileTypeAnalyzer" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.IO.Base.BaseFileTypeAnalyzer" />
    public class FileTypeAnalyzer: BaseFileTypeAnalyzer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileTypeAnalyzer" /> class.
        /// </summary>
        public FileTypeAnalyzer()
            : base(new StreamReader(
                Assembly.GetAssembly(typeof(FileTypeAnalyzer))
                  .GetManifestResourceStream("FileTypeDefinitions.json"))
                  .ReadToEnd()){ }
    }
}
