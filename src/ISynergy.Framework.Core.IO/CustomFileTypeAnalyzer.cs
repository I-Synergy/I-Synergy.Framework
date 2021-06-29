using ISynergy.Framework.Core.IO.Base;
using System.IO;
using System.Text;

namespace ISynergy.Framework.Core.IO
{
    /// <summary>
    /// Class CustomFileTypeAnalyzer.
    /// Implements the <see cref="ISynergy.Framework.Core.IO.Base.BaseFileTypeAnalyzer" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.IO.Base.BaseFileTypeAnalyzer" />
    public class CustomFileTypeAnalyzer : BaseFileTypeAnalyzer
    {
        /// <summary>
        /// Initializes a <see cref="CustomFileTypeAnalyzer" /> with the provided definitions file contents.
        /// </summary>
        /// <param name="definitionsFile">The json object representing the definitions file.</param>
        public CustomFileTypeAnalyzer(string definitionsFile)
            : base(definitionsFile)
        {
        }

        /// <summary>
        /// Initializes a <see cref="CustomFileTypeAnalyzer" /> with the definitions at the provided file path.
        /// </summary>
        /// <param name="filePath">Definitions file path.</param>
        /// <param name="encoding">The encoding.</param>
        public CustomFileTypeAnalyzer(string filePath, Encoding encoding)
            : base(File.ReadAllText(filePath, encoding))
        {
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
}
