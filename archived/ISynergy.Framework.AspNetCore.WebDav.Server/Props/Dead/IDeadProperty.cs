using System.Xml.Linq;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead
{
    /// <summary>
    /// The interface for a dead (read-only) property
    /// </summary>
    public interface IDeadProperty : IUntypedReadableProperty, IInitializableProperty
    {
        /// <summary>
        /// Determines if the returned value is the default value and therefore doesn't need to be copied.
        /// </summary>
        /// <param name="element">The element with the value to check if it has the default value</param>
        /// <returns><see langword="true"/> when the underlying is not the default value</returns>
        bool IsDefaultValue(XElement element);
    }
}
