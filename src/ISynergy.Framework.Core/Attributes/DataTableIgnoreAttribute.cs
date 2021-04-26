using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Core.Attributes
{
    /// <summary>
    /// Class DataTableIgnoreAttribute.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DataTableIgnoreAttribute : Attribute
    {
    }
}
