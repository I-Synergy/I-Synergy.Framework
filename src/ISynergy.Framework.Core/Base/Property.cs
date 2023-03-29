using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Validation;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Base
{
#nullable disable
    /// <summary>
    /// Class Property.
    /// Implements the <see cref="IProperty{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IProperty{T}" />
    public class Property<T> : IProperty<T>
    {
        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// The is original set
        /// </summary>
        private bool _IsOriginalSet = false;
        /// <summary>
        /// The is dirty
        /// </summary>
        private bool _IsDirty = true;
        /// <summary>
        /// The value
        /// </summary>
        private T _Value = default;
        /// <summary>
        /// The original value
        /// </summary>
        private T _OriginalValue = default;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property{T}"/> class.
        /// </summary>
        public Property(string name)
        {
            Argument.IsNotNullOrEmpty(name);

            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property{T}"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value">The value.</param>
        public Property(string name, T value)
            : this(name)
        {
            Value = value;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        public bool IsDirty
        {
            get { return _IsDirty; }
            private set { Set(ref _IsDirty, value); }
        }

        /// <summary>
        /// Name of the property.
        /// </summary>
        [JsonIgnore]
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get { return _Value; }
            set
            {
                if (!IsOriginalSet)
                {
                    OriginalValue = value;
                }

                Set(ref _Value, value);
                IsDirty = true;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is original set.
        /// </summary>
        /// <value><c>true</c> if this instance is original set; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        public bool IsOriginalSet
        {
            get { return _IsOriginalSet; }
            private set { Set(ref _IsOriginalSet, value); }
        }

        /// <summary>
        /// Gets or sets the original value.
        /// </summary>
        /// <value>The original value.</value>
        [JsonIgnore]
        public T OriginalValue
        {
            get { return _OriginalValue; }
            set
            {
                IsOriginalSet = true;
                Set(ref _OriginalValue, value);
            }
        }

        /// <summary>
        /// Sets the specified storage.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="storage">The storage.</param>
        /// <param name="value">The value.</param>
        private static void Set<V>(ref V storage, V value)
        {
            if (!Equals(storage, value))
            {
                storage = value;
            }
        }

        /// <summary>
        /// Resets the changes.
        /// </summary>
        public void ResetChanges()
        {
            Value = OriginalValue;
            IsDirty = false;
        }

        /// <summary>
        /// Marks as clean.
        /// </summary>
        public void MarkAsClean()
        {
            OriginalValue = Value;
            IsDirty = false;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString() => Value.ToString();
    }
}
