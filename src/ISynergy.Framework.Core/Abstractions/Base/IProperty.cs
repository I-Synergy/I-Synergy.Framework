using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Core.Abstractions.Base
{
    /// <summary>
    /// Interface IProperty
    /// Implements the <see cref="IBindable" />
    /// </summary>
    /// <seealso cref="IBindable" />
    public interface IProperty : IBindable
    {
        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        event EventHandler ValueChanged;

        /// <summary>
        /// Resets the changes.
        /// </summary>
        void ResetChanges();

        /// <summary>
        /// Marks as clean.
        /// </summary>
        void MarkAsClean();

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        ObservableCollection<string> Errors { get; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        bool IsValid { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        bool IsDirty { get; }

        /// <summary>
        /// Gets or sets the Broadcast on changes.
        /// </summary>
        [JsonIgnore]
        bool BroadCastChanges { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is original set.
        /// </summary>
        /// <value><c>true</c> if this instance is original set; otherwise, <c>false</c>.</value>
        bool IsOriginalSet { get; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        string Name { get; }
    }
}
