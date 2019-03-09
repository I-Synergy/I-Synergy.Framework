using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Background.Tasks.Units
{
    public interface ITimeRestrictableUnit
    {
        /// <summary>
        /// The schedule being affected.
        /// </summary>
        Schedule Schedule { get; }
    }
}
