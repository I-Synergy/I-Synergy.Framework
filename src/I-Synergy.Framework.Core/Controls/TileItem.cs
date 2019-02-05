using ISynergy.Models.Base;

namespace ISynergy.Controls
{
    /// <summary>
    /// TileItem model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public class TileItem : ModelBase
    {
        /// <summary>
        /// Gets or sets the Name property value.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Group property value.
        /// </summary>
        public string Group
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Description property value.
        /// </summary>
        public string Description
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Details property value.
        /// </summary>
        public string Details
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Header property value.
        /// </summary>
        public string Header
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Quantity property value.
        /// </summary>
        public string Quantity
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Source property value.
        /// </summary>
        public string Source
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsEnabled property value.
        /// </summary>
        public bool IsEnabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsTransparent property value.
        /// </summary>
        public bool IsTransparent
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsTimer property value.
        /// </summary>
        public bool IsTimer
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Wallpaper property value.
        /// </summary>
        public byte[] Wallpaper
        {
            get { return GetValue<byte[]>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Tag property value.
        /// </summary>
        public object Tag
        {
            get { return GetValue<object>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Height property value.
        /// </summary>
        public double Height
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Width property value.
        /// </summary>
        public double Width
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Color property value.
        /// </summary>
        public int Color
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
    }
}
