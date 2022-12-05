using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Validation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The is original set
        /// </summary>
        private bool _IsOriginalSet = false;
        /// <summary>
        /// The is dirty
        /// </summary>
        private bool _IsDirty = true;
        /// <summary>
        /// If set to true, changes will be broadcasted to the MessageService.
        /// </summary>
        private bool _broadCastChanges = false;
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
            Errors = new ObservableCollection<string>();
            Errors.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsValid));
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
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        [JsonIgnore]
        public ObservableCollection<string> Errors { get; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        public bool IsValid => !Errors.Any();

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
        /// If set to true, changes will be broadcasted to the MessageService.
        /// </summary>
        [JsonIgnore]
        public bool BroadCastChanges
        {
            get { return _broadCastChanges; }
            set { _broadCastChanges = value; }
        }

        /// <summary>
        /// Name of the property.
        /// </summary>
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
        public bool IsOriginalSet
        {
            get { return _IsOriginalSet; }
            private set { Set(ref _IsOriginalSet, value); }
        }

        /// <summary>
        /// Gets or sets the original value.
        /// </summary>
        /// <value>The original value.</value>
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
        /// <param name="callerMemberName">Name of the caller member.</param>
        private void Set<V>(ref V storage, V value, [CallerMemberName] string callerMemberName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                OnPropertyChanged(callerMemberName);
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

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
