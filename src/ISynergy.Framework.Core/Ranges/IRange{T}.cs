using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Core.Ranges
{
    /// <summary>
    ///   Common interface for Range objects, such as <see cref="DoubleRange"/>,
    ///   <see cref="IntRange"/> or <see cref="Range"/>. A range represents the
    ///   interval between two values in the form [min, max].
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the range.</typeparam>
    /// 
    /// <seealso cref="Range"/>
    /// <seealso cref="DoubleRange"/>
    /// <seealso cref="IntRange"/>
    /// <seealso cref="ByteRange"/>
    /// 
    public interface IRange<T> : IFormattable
    {
        /// <summary>
        ///   Minimum value of the range.
        /// </summary>
        /// 
        /// <remarks>
        ///   Represents minimum value (left side limit) of the range [<b>min</b>, max].
        /// </remarks>
        /// 
        T Min { get; set; }

        /// <summary>
        ///   Maximum value of the range.
        /// </summary>
        /// 
        /// <remarks>
        ///   Represents maximum value (right side limit) of the range [min, <b>max</b>].
        /// </remarks>
        /// 
        T Max { get; set; }
    }
}
