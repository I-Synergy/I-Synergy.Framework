using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Controls
{
    /// <summary>
    /// The <see cref="Expander"/> control allows user to show/hide content based on a boolean state
    /// </summary>
    public partial class Expander
    {
        /// <summary>
        /// Fires when the expander is opened
        /// </summary>
        public event EventHandler Expanded;

        /// <summary>
        /// Fires when the expander is closed
        /// </summary>
        public event EventHandler Collapsed;
    }
}
