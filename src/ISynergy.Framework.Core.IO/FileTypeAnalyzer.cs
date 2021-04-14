using ISynergy.Framework.Core.IO.Abtractions;
using ISynergy.Framework.Core.IO.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ISynergy.Framework.Core.IO
{
    /// <summary>
    /// Class FileTypeAnalyzer.
    /// Implements the <see cref="ISynergy.Framework.Core.IO.Base.BaseFileTypeAnalyzer" />
    /// Implements the <see cref="ISynergy.Framework.Core.IO.Abtractions.IFileTypeAnalyzer" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Core.IO.Base.BaseFileTypeAnalyzer" />
    /// <seealso cref="ISynergy.Framework.Core.IO.Abtractions.IFileTypeAnalyzer" />
    public class FileTypeAnalyzer: BaseFileTypeAnalyzer, IFileTypeAnalyzer
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
