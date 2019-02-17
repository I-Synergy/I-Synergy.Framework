using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Fixtures
{
    public class ModelFixture<T> : ModelBase, IDisposable
    {
        public ModelFixture()
            : base()
        {
        }

        public ModelFixture(T initialValue)
            : this()
        {
            Value = initialValue;
        }

        /// <summary>
        /// Gets or sets the Value property value.
        /// </summary>
        public T Value
        {
            get { return GetValue<T>(); }
            set { SetValue(value); }
        }

        public override string ToString()
        {
            if (Value == null)
            {
                return string.Empty;
            }
            else
            {
                return Value.ToString();
            }
        }
    }
}
