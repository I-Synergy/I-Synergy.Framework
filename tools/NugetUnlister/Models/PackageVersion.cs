using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Base;
using System;

namespace NugetUnlister.Models
{
    public class PackageVersion : ObservableClass
    {

        /// <summary>
        /// Gets or sets the Id property value.
        /// </summary>
        [Identity]
        public Guid Id
        {
            get => GetValue<Guid>();
            private set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the PackageId property value.
        /// </summary>
        public string PackageId
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the Version property value.
        /// </summary>
        public string Version
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the Selected property value.
        /// </summary>
        public bool Selected
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PackageVersion(string packageId, string version, bool selected)
        {
            Id = Guid.NewGuid();
            PackageId = packageId;
            Version = version;
            Selected = selected;
        }
    }
}
