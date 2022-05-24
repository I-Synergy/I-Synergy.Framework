using System;

namespace ISynergy.Framework.Core.Attributes
{
    /// <summary>
    /// Class SymbolAttribute.
    /// Implements the <see cref="Attribute" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SymbolAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the Symbol.
        /// </summary>
        /// <value>The description.</value>
        public string Symbol { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolAttribute"/> class.
        /// </summary>
        /// <param name="symbol">The description.</param>
        public SymbolAttribute(string symbol)
        {
            Symbol = symbol;
        }
    }
}
