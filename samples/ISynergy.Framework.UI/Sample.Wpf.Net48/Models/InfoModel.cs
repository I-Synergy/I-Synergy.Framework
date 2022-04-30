using ISynergy.Framework.Core.Base;
using System;

namespace Sample.Models
{
    public class InfoModel : ObservableClass
    {
        /// <summary>
        /// Gets or sets the CompanyName property value.
        /// </summary>
        /// <value>The name of the company.</value>
        public string CompanyName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ProductName property value.
        /// </summary>
        /// <value>The name of the product.</value>
        public string ProductName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Version property value.
        /// </summary>
        /// <value>The version.</value>
        public Version Version
        {
            get { return GetValue<Version>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Copyrights property value.
        /// </summary>
        /// <value>The copyrights.</value>
        public string Copyrights
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
