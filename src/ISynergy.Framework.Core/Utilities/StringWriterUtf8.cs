using System.Text;

namespace ISynergy.Framework.Core.Utilities;

/// <summary>
/// Class StringWriterUTF8.
/// Implements the <see cref="StringWriter" />
/// </summary>
/// <seealso cref="StringWriter" />
public class StringWriterUtf8 : StringWriter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringWriterUtf8"/> class.
    /// </summary>
    public StringWriterUtf8()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringWriterUtf8"/> class.
    /// </summary>
    /// <param name="formatProvider">An <see cref="T:System.IFormatProvider"></see> object that controls formatting.</param>
    public StringWriterUtf8(IFormatProvider formatProvider) : base(formatProvider)
    {
    }

    /// <summary>
    /// Gets the <see cref="T:System.Text.Encoding"></see> in which the output is written.
    /// </summary>
    /// <value>The encoding.</value>
    public override Encoding Encoding { get { return Encoding.UTF8; } }
}
