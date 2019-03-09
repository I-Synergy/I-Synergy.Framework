using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Background.Tasks.Units
{
    public interface IDayRestrictableUnit
    {
        /// <summary>
        /// The schedule being affected.
        /// </summary>
        Schedule Schedule { get; }

        /// <summary>
        /// Increment the given days.
        /// </summary>
        /// <param name="increment">Days to increment</param>
        /// <returns>The resulting date</returns>
        DateTime DayIncrement(DateTime increment);
    }
}
